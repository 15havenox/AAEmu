using System;
using System.Collections.Concurrent;
using Jitter2.Collision;
using Jitter2.Collision.Shapes;
using Jitter2.LinearMath;
using NLog;

namespace AAEmu.Game.Physics.HeightMaps;

/// <summary>
/// Sistema aprimorado de detecção de heightmap com cache e otimizações
/// </summary>
public class ImprovedHeightmapDetection : IBroadPhaseFilter
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    
    private readonly Jitter2.World _world;
    private readonly HeightmapTester _shape;
    private readonly Heightmap _heightmap;
    private readonly ulong _minIndex;
    
    // Cache para otimização de performance
    private readonly ConcurrentDictionary<(int, int), float> _heightCache = new();
    private readonly ConcurrentDictionary<(int, int), JVector> _normalCache = new();
    private readonly object _cacheLock = new();
    
    // Configurações
    private const int CacheMaxSize = 10000;
    private const float MinCollisionDistance = 0.1f;

    public ImprovedHeightmapDetection(Jitter2.World world, HeightmapTester shape)
    {
        _shape = shape;
        _world = world;
        _heightmap = shape.Heightmap;

        (_minIndex, _) = Jitter2.World.RequestId(_heightmap.Width * _heightmap.Height * 2);
    }

    public bool Filter(IDynamicTreeProxy shapeA, IDynamicTreeProxy shapeB)
    {
        if (shapeA != _shape && shapeB != _shape) 
            return true;

        var collider = shapeA == _shape ? shapeB : shapeA;

        if (collider is not RigidBodyShape rbs || rbs.RigidBody.Data.IsStaticOrInactive) 
            return false;

        try
        {
            ProcessCollision(rbs);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error processing heightmap collision");
        }

        return false;
    }

    private void ProcessCollision(RigidBodyShape rbs)
    {
        ref var body = ref rbs.RigidBody!.Data;

        var boundingBox = rbs.WorldBoundingBox;
        var (minX, minZ, maxX, maxZ) = GetCollisionBounds(boundingBox);

        for (var x = minX; x < maxX; x++)
        {
            for (var z = minZ; z < maxZ; z++)
            {
                ProcessQuadCollision(x, z, rbs, body);
            }
        }
    }

    private (int minX, int minZ, int maxX, int maxZ) GetCollisionBounds(BoundingBox boundingBox)
    {
        var minX = Math.Max(0, (int)Math.Floor(boundingBox.Min.X));
        var minZ = Math.Max(0, (int)Math.Floor(boundingBox.Min.Z));
        var maxX = Math.Min(_heightmap.Width - 1, (int)Math.Ceiling(boundingBox.Max.X));
        var maxZ = Math.Min(_heightmap.Height - 1, (int)Math.Ceiling(boundingBox.Max.Z));
        
        return (minX, minZ, maxX, maxZ);
    }

    private void ProcessQuadCollision(int x, int z, RigidBodyShape rbs, in RigidBodyData body)
    {
        // Primeiro triângulo do quad
        var triangle1 = GetTriangle1(x, z);
        ProcessTriangleCollision(triangle1, rbs, body, GetTriangleIndex(x, z, 0));

        // Segundo triângulo do quad
        var triangle2 = GetTriangle2(x, z);
        ProcessTriangleCollision(triangle2, rbs, body, GetTriangleIndex(x, z, 1));
    }

    private CollisionTriangle GetTriangle1(int x, int z)
    {
        return new CollisionTriangle
        {
            A = new JVector(x + 0, GetCachedHeight(x + 0, z + 0), z + 0),
            B = new JVector(x + 1, GetCachedHeight(x + 1, z + 0), z + 0),
            C = new JVector(x + 1, GetCachedHeight(x + 1, z + 1), z + 1)
        };
    }

    private CollisionTriangle GetTriangle2(int x, int z)
    {
        return new CollisionTriangle
        {
            A = new JVector(x + 0, GetCachedHeight(x + 0, z + 0), z + 0),
            B = new JVector(x + 1, GetCachedHeight(x + 1, z + 1), z + 1),
            C = new JVector(x + 0, GetCachedHeight(x + 0, z + 1), z + 1)
        };
    }

    private void ProcessTriangleCollision(CollisionTriangle triangle, RigidBodyShape rbs, in RigidBodyData body, ulong triangleIndex)
    {
        var normal = GetCachedNormal(triangle);
        
        var hit = NarrowPhase.MprEpa(triangle, rbs, body.Orientation, body.Position, 
            out var pointA, out var pointB, out _, out var penetration);

        if (hit && penetration > MinCollisionDistance)
        {
            _world.RegisterContact(rbs.ShapeId, triangleIndex, _world.NullBody, rbs.RigidBody, 
                pointA, pointB, normal);
        }
    }

    private float GetCachedHeight(int x, int z)
    {
        var key = (x, z);
        
        if (_heightCache.TryGetValue(key, out var cachedHeight))
            return cachedHeight;

        lock (_cacheLock)
        {
            // Double-check locking
            if (_heightCache.TryGetValue(key, out cachedHeight))
                return cachedHeight;

            // Limpar cache se necessário
            if (_heightCache.Count >= CacheMaxSize)
            {
                ClearOldCacheEntries();
            }

            var height = _heightmap.GetHeight(x, z);
            _heightCache[key] = height;
            return height;
        }
    }

    private JVector GetCachedNormal(CollisionTriangle triangle)
    {
        // Usar posição central do triângulo como chave
        var centerX = (int)((triangle.A.X + triangle.B.X + triangle.C.X) / 3);
        var centerZ = (int)((triangle.A.Z + triangle.B.Z + triangle.C.Z) / 3);
        var key = (centerX, centerZ);

        if (_normalCache.TryGetValue(key, out var cachedNormal))
            return cachedNormal;

        lock (_cacheLock)
        {
            if (_normalCache.TryGetValue(key, out cachedNormal))
                return cachedNormal;

            if (_normalCache.Count >= CacheMaxSize)
            {
                ClearOldNormalCacheEntries();
            }

            var normal = CalculateTriangleNormal(triangle);
            _normalCache[key] = normal;
            return normal;
        }
    }

    private JVector CalculateTriangleNormal(CollisionTriangle triangle)
    {
        var edge1 = triangle.B - triangle.A;
        var edge2 = triangle.C - triangle.A;
        var normal = JVector.Cross(edge1, edge2);
        
        // Garantir que a normal aponte para cima
        if (normal.Y < 0)
            normal = -normal;
            
        return JVector.Normalize(normal);
    }

    private ulong GetTriangleIndex(int x, int z, int triangleInQuad)
    {
        return _minIndex + (ulong)(2 * (x * _heightmap.Width + z) + triangleInQuad);
    }

    private void ClearOldCacheEntries()
    {
        // Limpar metade do cache para dar espaço
        var entriesToRemove = _heightCache.Count / 2;
        var keys = _heightCache.Keys.Take(entriesToRemove).ToList();
        
        foreach (var key in keys)
        {
            _heightCache.TryRemove(key, out _);
        }
    }

    private void ClearOldNormalCacheEntries()
    {
        var entriesToRemove = _normalCache.Count / 2;
        var keys = _normalCache.Keys.Take(entriesToRemove).ToList();
        
        foreach (var key in keys)
        {
            _normalCache.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Limpa todos os caches
    /// </summary>
    public void ClearCache()
    {
        lock (_cacheLock)
        {
            _heightCache.Clear();
            _normalCache.Clear();
        }
    }

    /// <summary>
    /// Obtém estatísticas do cache
    /// </summary>
    public (int heightCacheSize, int normalCacheSize) GetCacheStats()
    {
        return (_heightCache.Count, _normalCache.Count);
    }
}