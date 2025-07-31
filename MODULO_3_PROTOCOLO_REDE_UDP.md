# MÓDULO 3: PROTOCOLO DE REDE CUSTOMIZADO SOBRE UDP

---

## Introdução ao Módulo 3

Neste módulo, vamos desenvolver um protocolo de rede customizado sobre UDP especificamente otimizado para MMORPGs. Este é o coração da comunicação cliente-servidor e determinará a qualidade da experiência de jogo.

**Por que um protocolo customizado é necessário para MMORPGs?**

1. **Performance**: UDP puro é rápido, mas não confiável
2. **Controle**: Precisamos de controle total sobre priorização e compressão
3. **Segurança**: Proteção contra cheating e ataques
4. **Escalabilidade**: Otimizado para milhares de conexões simultâneas
5. **Flexibilidade**: Adaptável às necessidades específicas do jogo

---

## 3.1 FUNDAMENTOS DE REDES PARA JOGOS

### TCP vs UDP: A Decisão Fundamental

#### TCP (Transmission Control Protocol)

**Características:**
```
✅ Vantagens:
- Confiabilidade garantida
- Ordem de entrega garantida
- Controle de fluxo automático
- Detecção de erros
- Amplamente suportado

❌ Desvantagens:
- Latência alta (handshake de 3 vias)
- Head-of-line blocking
- Overhead de headers
- Retransmissão automática pode causar lag spikes
- Não adequado para dados em tempo real
```

**Exemplo de problema com TCP em jogos:**
```
Cenário: Player pressiona botão para atacar

TCP Sequence:
1. Client envia "Attack" packet
2. Packet 1 se perde na rede
3. Client envia "Move" packet  
4. Server recebe "Move" mas espera "Attack"
5. Server não processa "Move" até "Attack" chegar
6. Player experimenta lag artificial

Resultado: Input lag desnecessário devido ao head-of-line blocking
```

#### UDP (User Datagram Protocol)

**Características:**
```
✅ Vantagens:
- Latência mínima (sem handshake)
- Sem head-of-line blocking
- Headers pequenos (8 bytes vs 20+ do TCP)
- Controle total sobre retransmissão
- Ideal para dados em tempo real

❌ Desvantagens:
- Sem garantia de entrega
- Sem garantia de ordem
- Sem controle de fluxo
- Sem detecção de erros
- Requer implementação customizada
```

#### Por que UDP para MMORPGs?

**Análise de Requisitos:**
```
Dados Críticos (precisam de confiabilidade):
- Login/Authentication
- Inventory changes
- Character stats
- Trade transactions
- Guild operations

Dados em Tempo Real (precisam de velocidade):
- Player movement
- Combat actions
- Chat messages
- World updates
- Animation states

Solução: UDP + Reliable Layer Customizada
- Velocidade do UDP para dados tempo real
- Confiabilidade seletiva para dados críticos
- Controle total sobre priorização
```

### Conceitos Fundamentais de Networking

#### Latência e seus Componentes

**Latência Total = Network Latency + Processing Latency + Queuing Latency**

```csharp
// Exemplo de medição de latência
public class LatencyMeasurement
{
    private readonly Dictionary<uint, DateTime> _sentPackets = new();
    private readonly Queue<float> _latencyHistory = new();
    private const int HistorySize = 100;

    public void SendPing(uint sequenceId)
    {
        _sentPackets[sequenceId] = DateTime.UtcNow;
        
        var pingPacket = new NetworkPacket(
            PacketType.Heartbeat, 
            sequenceId, 
            BitConverter.GetBytes(DateTime.UtcNow.Ticks)
        );
        
        SendPacket(pingPacket);
    }

    public void ProcessPong(uint sequenceId)
    {
        if (_sentPackets.TryGetValue(sequenceId, out var sentTime))
        {
            var latency = (float)(DateTime.UtcNow - sentTime).TotalMilliseconds;
            
            _latencyHistory.Enqueue(latency);
            if (_latencyHistory.Count > HistorySize)
                _latencyHistory.Dequeue();
            
            _sentPackets.Remove(sequenceId);
            
            // Log latency spikes
            if (latency > 150) // 150ms threshold
            {
                Logger.Warning($"High latency detected: {latency:F2}ms");
            }
        }
    }

    public float AverageLatency => _latencyHistory.Count > 0 ? 
        _latencyHistory.Average() : 0;
    
    public float LatencyVariation => _latencyHistory.Count > 1 ?
        CalculateStandardDeviation(_latencyHistory) : 0;
}

/*
Por que medir latência?
1. Adaptive algorithms: Ajustar timeouts baseado na latência atual
2. Quality metrics: Monitorar experiência do player
3. Server selection: Escolher servidor com menor latência
4. Lag compensation: Ajustar algoritmos de predição
*/
```

#### Packet Loss e Jitter

**Packet Loss:**
```csharp
public class PacketLossTracker
{
    private uint _expectedSequence = 0;
    private readonly HashSet<uint> _receivedPackets = new();
    private uint _totalPacketsExpected = 0;
    private uint _totalPacketsReceived = 0;

    public void ProcessPacket(uint sequenceId)
    {
        _receivedPackets.Add(sequenceId);
        _totalPacketsReceived++;
        
        // Update expected sequence
        if (sequenceId >= _expectedSequence)
        {
            // Calculate how many packets we expected up to this point
            var packetsExpected = sequenceId - _expectedSequence + 1;
            _totalPacketsExpected += packetsExpected;
            _expectedSequence = sequenceId + 1;
        }
    }

    public float PacketLossPercentage => _totalPacketsExpected > 0 ?
        (1.0f - (float)_totalPacketsReceived / _totalPacketsExpected) * 100 : 0;

    public bool IsPacketMissing(uint sequenceId) => 
        !_receivedPackets.Contains(sequenceId);
}

/*
Por que monitorar packet loss?
1. Network quality: Detectar problemas de conectividade
2. Adaptive bitrate: Reduzir dados enviados em redes ruins
3. Retransmission: Decidir quando reenviar pacotes
4. User experience: Alertar sobre problemas de conexão
*/
```

**Jitter (Variação de Latência):**
```csharp
public class JitterBuffer
{
    private readonly Queue<TimestampedPacket> _buffer = new();
    private readonly TimeSpan _bufferDelay;
    private DateTime _lastPlayoutTime = DateTime.MinValue;

    public JitterBuffer(TimeSpan bufferDelay)
    {
        _bufferDelay = bufferDelay;
    }

    public void AddPacket(NetworkPacket packet, DateTime arrivalTime)
    {
        var timestampedPacket = new TimestampedPacket
        {
            Packet = packet,
            ArrivalTime = arrivalTime,
            PlayoutTime = arrivalTime + _bufferDelay
        };

        // Insert in order of playout time
        InsertInOrder(timestampedPacket);
    }

    public NetworkPacket? GetNextPacket()
    {
        var now = DateTime.UtcNow;
        
        if (_buffer.Count > 0 && _buffer.Peek().PlayoutTime <= now)
        {
            var packet = _buffer.Dequeue();
            _lastPlayoutTime = packet.PlayoutTime;
            return packet.Packet;
        }
        
        return null;
    }

    private void InsertInOrder(TimestampedPacket packet)
    {
        // Simple implementation - for production, use priority queue
        var tempList = _buffer.ToList();
        tempList.Add(packet);
        tempList.Sort((a, b) => a.PlayoutTime.CompareTo(b.PlayoutTime));
        
        _buffer.Clear();
        foreach (var p in tempList)
            _buffer.Enqueue(p);
    }
}

/*
Por que usar jitter buffer?
1. Smooth playback: Compensa variação na chegada de pacotes
2. Reduce stuttering: Evita pausas causadas por packets atrasados
3. Order preservation: Mantém ordem correta de eventos
4. Adaptive sizing: Pode ajustar tamanho baseado na rede
*/
```

### Reliable UDP: O Melhor dos Dois Mundos

#### Conceito de Reliable UDP

**Reliable UDP = UDP + Selective Reliability**

```csharp
public enum ReliabilityType
{
    Unreliable,           // Fire and forget (movement updates)
    Reliable,             // Guaranteed delivery (inventory changes)
    ReliableOrdered,      // Guaranteed delivery + order (chat messages)
    ReliableSequenced     // Latest packet only (player stats)
}

public class ReliableUdpPacket
{
    public PacketType Type { get; set; }
    public ReliabilityType Reliability { get; set; }
    public uint SequenceNumber { get; set; }
    public uint AckNumber { get; set; }
    public uint ChannelId { get; set; }  // For ordering within channels
    public byte[] Data { get; set; }
    public DateTime SentTime { get; set; }
    public int RetryCount { get; set; }
}

/*
Por que diferentes tipos de confiabilidade?

1. Unreliable: 
   - Uso: Movement updates, heartbeats
   - Vantagem: Latência mínima
   - Trade-off: Pode perder dados

2. Reliable:
   - Uso: Inventory, stats, important events
   - Vantagem: Garantia de entrega
   - Trade-off: Latência ligeiramente maior

3. ReliableOrdered:
   - Uso: Chat messages, quest updates
   - Vantagem: Ordem garantida
   - Trade-off: Head-of-line blocking dentro do canal

4. ReliableSequenced:
   - Uso: Player stats que mudam frequentemente
   - Vantagem: Sempre a versão mais recente
   - Trade-off: Pacotes antigos são descartados
*/
```

#### Implementação Base do Reliable UDP

```csharp
public class ReliableUdpConnection
{
    private readonly UdpClient _udpClient;
    private readonly Dictionary<uint, ReliableUdpPacket> _pendingAcks = new();
    private readonly Dictionary<uint, ReliableUdpPacket> _receivedPackets = new();
    private readonly Timer _retransmissionTimer;
    
    private uint _nextSequenceNumber = 1;
    private uint _lastAckedSequence = 0;
    private readonly object _lockObject = new();

    public ReliableUdpConnection(UdpClient udpClient)
    {
        _udpClient = udpClient;
        _retransmissionTimer = new Timer(ProcessRetransmissions, null, 
            TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50));
    }

    public async Task SendAsync(byte[] data, ReliabilityType reliability, uint channelId = 0)
    {
        var packet = new ReliableUdpPacket
        {
            SequenceNumber = GetNextSequenceNumber(),
            Reliability = reliability,
            ChannelId = channelId,
            Data = data,
            SentTime = DateTime.UtcNow,
            RetryCount = 0
        };

        await SendPacketAsync(packet);

        // Store for potential retransmission
        if (reliability != ReliabilityType.Unreliable)
        {
            lock (_lockObject)
            {
                _pendingAcks[packet.SequenceNumber] = packet;
            }
        }
    }

    private async Task SendPacketAsync(ReliableUdpPacket packet)
    {
        var serialized = SerializePacket(packet);
        await _udpClient.SendAsync(serialized, serialized.Length);
        
        // Update metrics
        NetworkMetrics.PacketsSent.Increment();
        NetworkMetrics.BytesSent.Add(serialized.Length);
    }

    public void ProcessReceivedPacket(byte[] data)
    {
        var packet = DeserializePacket(data);
        
        // Send ACK if reliable
        if (packet.Reliability != ReliabilityType.Unreliable)
        {
            SendAck(packet.SequenceNumber);
        }

        // Process based on reliability type
        switch (packet.Reliability)
        {
            case ReliabilityType.Unreliable:
                ProcessUnreliablePacket(packet);
                break;
                
            case ReliabilityType.Reliable:
                ProcessReliablePacket(packet);
                break;
                
            case ReliabilityType.ReliableOrdered:
                ProcessReliableOrderedPacket(packet);
                break;
                
            case ReliabilityType.ReliableSequenced:
                ProcessReliableSequencedPacket(packet);
                break;
        }
    }

    private void ProcessRetransmissions(object state)
    {
        var now = DateTime.UtcNow;
        var packetsToRetry = new List<ReliableUdpPacket>();

        lock (_lockObject)
        {
            foreach (var kvp in _pendingAcks.ToList())
            {
                var packet = kvp.Value;
                var timeSinceSent = now - packet.SentTime;

                // Adaptive timeout based on RTT
                var timeout = CalculateRetransmissionTimeout();
                
                if (timeSinceSent > timeout)
                {
                    packet.RetryCount++;
                    packet.SentTime = now;

                    if (packet.RetryCount < MaxRetries)
                    {
                        packetsToRetry.Add(packet);
                    }
                    else
                    {
                        // Give up on this packet
                        _pendingAcks.Remove(kvp.Key);
                        OnPacketLost?.Invoke(packet);
                    }
                }
            }
        }

        // Retransmit outside of lock
        foreach (var packet in packetsToRetry)
        {
            _ = SendPacketAsync(packet);
        }
    }

    private TimeSpan CalculateRetransmissionTimeout()
    {
        // Simplified RTO calculation (RFC 6298)
        var smoothedRTT = _latencyMeasurement.AverageLatency;
        var rttVariation = _latencyMeasurement.LatencyVariation;
        
        var rto = smoothedRTT + Math.Max(100, 4 * rttVariation);
        return TimeSpan.FromMilliseconds(Math.Min(Math.Max(rto, 100), 3000));
    }
}

/*
Características importantes desta implementação:

1. Adaptive Timeout: RTO baseado na latência medida
2. Selective Retransmission: Apenas pacotes perdidos são reenviados
3. Channel-based Ordering: Ordem mantida por canal
4. Metrics Integration: Coleta métricas para monitoramento
5. Thread Safety: Locks apropriados para operações concorrentes
*/
```

---

## 3.2 DESENVOLVENDO O PROTOCOLO BASE

### Estrutura de Pacotes Customizada

#### Header Design

**Layout do Header (24 bytes):**
```
 0                   1                   2                   3
 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
|  Protocol ID  |    Version    |   Packet Type |   Reliability |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
|                        Sequence Number                        |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
|                          Timestamp                            |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
|                         Data Length                           |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
|                           Checksum                            |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
|                          Channel ID                           |
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```

```csharp
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PacketHeader
{
    public byte ProtocolId;        // Magic number for protocol identification
    public byte Version;           // Protocol version for compatibility
    public byte PacketType;        // Type of packet (see PacketType enum)
    public byte Reliability;       // Reliability flags
    public uint SequenceNumber;    // Sequence number for ordering/acks
    public uint Timestamp;         // Timestamp for RTT calculation
    public uint DataLength;        // Length of payload data
    public uint Checksum;          // CRC32 checksum for integrity
    public uint ChannelId;         // Channel for ordered delivery

    public const int Size = 24;
    public const byte MagicNumber = 0x4D; // 'M' for MMORPG
    public const byte CurrentVersion = 1;

    public bool IsValid =>
        ProtocolId == MagicNumber &&
        Version <= CurrentVersion &&
        DataLength <= MaxPacketSize;

    public const uint MaxPacketSize = 1400; // MTU safe size
}

/*
Por que cada campo?

1. ProtocolId: Identifica nosso protocolo vs outros dados UDP
2. Version: Permite evolução do protocolo
3. PacketType: Roteamento rápido no servidor
4. Reliability: Flags para comportamento de entrega
5. SequenceNumber: Essencial para ordering e ACKs
6. Timestamp: Medição de RTT e lag compensation
7. DataLength: Validação e parsing
8. Checksum: Detecção de corrupção
9. ChannelId: Ordering independente por canal
*/
```

#### Packet Types e Categorização

```csharp
public enum PacketType : byte
{
    // Connection Management (0x00-0x0F)
    Handshake = 0x00,
    HandshakeResponse = 0x01,
    Heartbeat = 0x02,
    Disconnect = 0x03,
    Acknowledgment = 0x04,
    
    // Authentication (0x10-0x1F)
    AuthRequest = 0x10,
    AuthResponse = 0x11,
    AuthChallenge = 0x12,
    AuthSuccess = 0x13,
    AuthFailure = 0x14,
    
    // Player Data (0x20-0x2F)
    PlayerMove = 0x20,
    PlayerAction = 0x21,
    PlayerState = 0x22,
    PlayerStats = 0x23,
    PlayerEquipment = 0x24,
    
    // World Data (0x30-0x3F)
    WorldState = 0x30,
    EntitySpawn = 0x31,
    EntityDespawn = 0x32,
    EntityUpdate = 0x33,
    EnvironmentUpdate = 0x34,
    
    // Game Systems (0x40-0x4F)
    InventoryUpdate = 0x40,
    CombatAction = 0x41,
    QuestUpdate = 0x42,
    ChatMessage = 0x43,
    TradeRequest = 0x44,
    
    // Administrative (0xF0-0xFF)
    ServerMessage = 0xF0,
    Error = 0xFE,
    Unknown = 0xFF
}

/*
Por que categorizar packets?

1. Routing: Servidores podem rotear baseado na categoria
2. Priority: Diferentes prioridades por categoria
3. Processing: Handlers específicos por categoria
4. Debugging: Mais fácil de debuggar por categoria
5. Extensibility: Fácil adicionar novos tipos
*/
```

### Sistema de Handshake

#### Handshake de 3 Vias Customizado

```csharp
public class HandshakeManager
{
    private readonly Dictionary<EndPoint, HandshakeState> _pendingHandshakes = new();
    private readonly SecureRandom _random = new();

    public enum HandshakePhase
    {
        None,
        ClientHelloSent,
        ServerHelloReceived,
        ClientFinishSent,
        Completed
    }

    public class HandshakeState
    {
        public HandshakePhase Phase { get; set; }
        public uint ClientNonce { get; set; }
        public uint ServerNonce { get; set; }
        public byte[] SharedSecret { get; set; }
        public DateTime StartTime { get; set; }
        public int RetryCount { get; set; }
    }

    // Client-side handshake initiation
    public async Task<bool> InitiateHandshakeAsync(EndPoint serverEndpoint)
    {
        var state = new HandshakeState
        {
            Phase = HandshakePhase.ClientHelloSent,
            ClientNonce = _random.NextUInt32(),
            StartTime = DateTime.UtcNow
        };

        _pendingHandshakes[serverEndpoint] = state;

        var clientHello = new HandshakePacket
        {
            Type = HandshakeType.ClientHello,
            ClientNonce = state.ClientNonce,
            ProtocolVersion = PacketHeader.CurrentVersion,
            SupportedFeatures = GetSupportedFeatures()
        };

        await SendHandshakePacket(serverEndpoint, clientHello);
        return await WaitForHandshakeCompletion(serverEndpoint);
    }

    // Server-side handshake processing
    public async Task<HandshakeResult> ProcessHandshakePacket(
        EndPoint clientEndpoint, HandshakePacket packet)
    {
        switch (packet.Type)
        {
            case HandshakeType.ClientHello:
                return await ProcessClientHello(clientEndpoint, packet);
                
            case HandshakeType.ClientFinish:
                return await ProcessClientFinish(clientEndpoint, packet);
                
            default:
                return HandshakeResult.InvalidPacket;
        }
    }

    private async Task<HandshakeResult> ProcessClientHello(
        EndPoint clientEndpoint, HandshakePacket packet)
    {
        // Validate protocol version
        if (packet.ProtocolVersion != PacketHeader.CurrentVersion)
        {
            await SendHandshakeError(clientEndpoint, 
                HandshakeError.UnsupportedVersion);
            return HandshakeResult.VersionMismatch;
        }

        // Generate server nonce and shared secret
        var serverNonce = _random.NextUInt32();
        var sharedSecret = GenerateSharedSecret(packet.ClientNonce, serverNonce);

        var state = new HandshakeState
        {
            Phase = HandshakePhase.ServerHelloReceived,
            ClientNonce = packet.ClientNonce,
            ServerNonce = serverNonce,
            SharedSecret = sharedSecret,
            StartTime = DateTime.UtcNow
        };

        _pendingHandshakes[clientEndpoint] = state;

        var serverHello = new HandshakePacket
        {
            Type = HandshakeType.ServerHello,
            ClientNonce = packet.ClientNonce,
            ServerNonce = serverNonce,
            SharedSecretHash = ComputeHash(sharedSecret),
            SupportedFeatures = GetSupportedFeatures()
        };

        await SendHandshakePacket(clientEndpoint, serverHello);
        return HandshakeResult.Pending;
    }

    private async Task<HandshakeResult> ProcessClientFinish(
        EndPoint clientEndpoint, HandshakePacket packet)
    {
        if (!_pendingHandshakes.TryGetValue(clientEndpoint, out var state))
        {
            return HandshakeResult.InvalidState;
        }

        // Verify client computed correct shared secret
        var expectedHash = ComputeHash(state.SharedSecret);
        if (!packet.SharedSecretHash.SequenceEqual(expectedHash))
        {
            _pendingHandshakes.Remove(clientEndpoint);
            return HandshakeResult.AuthenticationFailed;
        }

        // Handshake completed successfully
        state.Phase = HandshakePhase.Completed;
        
        // Store connection state for future packets
        RegisterConnection(clientEndpoint, state.SharedSecret);
        
        var serverFinish = new HandshakePacket
        {
            Type = HandshakeType.ServerFinish,
            SessionId = GenerateSessionId(),
            ConnectionEstablished = true
        };

        await SendHandshakePacket(clientEndpoint, serverFinish);
        _pendingHandshakes.Remove(clientEndpoint);
        
        return HandshakeResult.Success;
    }

    private byte[] GenerateSharedSecret(uint clientNonce, uint serverNonce)
    {
        // Simple shared secret generation - in production use proper key exchange
        var combined = new byte[8];
        BitConverter.GetBytes(clientNonce).CopyTo(combined, 0);
        BitConverter.GetBytes(serverNonce).CopyTo(combined, 4);
        
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(combined);
    }
}

/*
Por que handshake customizado?

1. Protocol Negotiation: Cliente e servidor acordam capacidades
2. Nonce Exchange: Previne replay attacks
3. Shared Secret: Base para criptografia subsequente
4. Version Check: Compatibilidade entre versões
5. Feature Detection: Negocia funcionalidades opcionais
*/
```

### Heartbeat e Detecção de Conexão

```csharp
public class ConnectionManager
{
    private readonly Dictionary<EndPoint, ConnectionState> _connections = new();
    private readonly Timer _heartbeatTimer;
    private readonly Timer _timeoutTimer;

    public class ConnectionState
    {
        public DateTime LastHeartbeat { get; set; }
        public DateTime LastPacketReceived { get; set; }
        public uint LastSequenceNumber { get; set; }
        public bool IsConnected { get; set; }
        public TimeSpan RTT { get; set; }
        public float PacketLoss { get; set; }
    }

    public ConnectionManager()
    {
        _heartbeatTimer = new Timer(SendHeartbeats, null,
            TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        _timeoutTimer = new Timer(CheckTimeouts, null,
            TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    private void SendHeartbeats(object state)
    {
        var now = DateTime.UtcNow;
        
        foreach (var kvp in _connections.ToList())
        {
            var endpoint = kvp.Key;
            var connection = kvp.Value;
            
            if (!connection.IsConnected) continue;
            
            // Send heartbeat every second
            if (now - connection.LastHeartbeat > TimeSpan.FromSeconds(1))
            {
                var heartbeat = new NetworkPacket
                {
                    Type = PacketType.Heartbeat,
                    SequenceNumber = GetNextSequenceNumber(endpoint),
                    Timestamp = (uint)now.Ticks,
                    Data = BitConverter.GetBytes(now.Ticks)
                };

                SendPacket(endpoint, heartbeat);
                connection.LastHeartbeat = now;
            }
        }
    }

    private void CheckTimeouts(object state)
    {
        var now = DateTime.UtcNow;
        var timeoutThreshold = TimeSpan.FromSeconds(30);
        
        foreach (var kvp in _connections.ToList())
        {
            var endpoint = kvp.Key;
            var connection = kvp.Value;
            
            if (connection.IsConnected && 
                now - connection.LastPacketReceived > timeoutThreshold)
            {
                // Connection timed out
                connection.IsConnected = false;
                OnConnectionLost?.Invoke(endpoint, DisconnectReason.Timeout);
                
                Logger.Warning($"Connection timeout: {endpoint}");
            }
        }
    }

    public void ProcessHeartbeat(EndPoint endpoint, NetworkPacket packet)
    {
        if (_connections.TryGetValue(endpoint, out var connection))
        {
            connection.LastPacketReceived = DateTime.UtcNow;
            
            // Calculate RTT from heartbeat
            if (packet.Data.Length >= 8)
            {
                var sentTicks = BitConverter.ToInt64(packet.Data, 0);
                var sentTime = new DateTime(sentTicks);
                connection.RTT = DateTime.UtcNow - sentTime;
            }
            
            // Send heartbeat response
            var response = new NetworkPacket
            {
                Type = PacketType.Heartbeat,
                SequenceNumber = packet.SequenceNumber, // Echo sequence
                Timestamp = packet.Timestamp,
                Data = packet.Data // Echo data
            };
            
            SendPacket(endpoint, response);
        }
    }
}

/*
Por que heartbeat é importante?

1. Connection Detection: Detecta conexões perdidas rapidamente
2. RTT Measurement: Mede latência continuamente
3. NAT Keepalive: Mantém NAT/firewall holes abertos
4. Quality Metrics: Coleta dados de qualidade da conexão
5. Load Balancing: Dados para decisões de balanceamento
*/
```

---

## 3.3 IMPLEMENTANDO CONFIABILIDADE

### Sistema de Acknowledgment (ACK)

#### ACK Packet Design

```csharp
public class AcknowledgmentSystem
{
    private readonly Dictionary<EndPoint, AckState> _ackStates = new();
    
    public class AckState
    {
        public uint LastAckedSequence { get; set; }
        public BitArray ReceivedPackets { get; set; } = new(1024);
        public Queue<uint> RecentAcks { get; set; } = new();
        public DateTime LastAckSent { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AckPacket
    {
        public uint LastReceivedSequence;    // Highest sequence received
        public uint AckBitfield;             // Bitmask of recently received packets
        public ushort PacketCount;           // Number of packets acknowledged
        public ushort Reserved;              // Future use
        
        public const int Size = 12;
    }

    public void ProcessReceivedPacket(EndPoint sender, uint sequenceNumber)
    {
        if (!_ackStates.TryGetValue(sender, out var state))
        {
            state = new AckState();
            _ackStates[sender] = state;
        }

        // Mark packet as received
        var index = (int)(sequenceNumber % 1024);
        state.ReceivedPackets[index] = true;
        state.RecentAcks.Enqueue(sequenceNumber);

        // Keep only recent ACKs (last 32)
        while (state.RecentAcks.Count > 32)
            state.RecentAcks.Dequeue();

        // Update highest sequence if newer
        if (sequenceNumber > state.LastAckedSequence)
        {
            state.LastAckedSequence = sequenceNumber;
        }

        // Send ACK immediately for reliable packets
        SendAcknowledgment(sender, state);
    }

    private void SendAcknowledgment(EndPoint recipient, AckState state)
    {
        var ackPacket = new AckPacket
        {
            LastReceivedSequence = state.LastAckedSequence,
            AckBitfield = BuildAckBitfield(state),
            PacketCount = (ushort)state.RecentAcks.Count
        };

        var packet = new NetworkPacket
        {
            Type = PacketType.Acknowledgment,
            SequenceNumber = 0, // ACKs don't need sequence numbers
            Data = StructToBytes(ackPacket)
        };

        SendPacket(recipient, packet);
        state.LastAckSent = DateTime.UtcNow;
    }

    private uint BuildAckBitfield(AckState state)
    {
        uint bitfield = 0;
        var recentAcks = state.RecentAcks.ToArray();
        
        // Build bitmask for last 32 packets relative to highest sequence
        for (int i = 0; i < Math.Min(32, recentAcks.Length); i++)
        {
            var sequenceOffset = state.LastAckedSequence - recentAcks[i];
            if (sequenceOffset < 32)
            {
                bitfield |= (1u << (int)sequenceOffset);
            }
        }
        
        return bitfield;
    }

    public void ProcessAcknowledgment(EndPoint sender, AckPacket ackPacket)
    {
        // Remove acknowledged packets from pending retransmission
        for (uint seq = ackPacket.LastReceivedSequence; seq > 0; seq--)
        {
            var offset = ackPacket.LastReceivedSequence - seq;
            if (offset >= 32) break;
            
            if ((ackPacket.AckBitfield & (1u << (int)offset)) != 0)
            {
                RemovePendingPacket(sender, seq);
            }
        }
    }
}

/*
Por que sistema de ACK eficiente?

1. Batch ACKs: Um ACK confirma múltiplos pacotes
2. Bitfield: Compacto para representar gaps
3. Selective ACK: Apenas pacotes perdidos são reenviados
4. Low Overhead: ACKs são pequenos e eficientes
5. Fast Recovery: Detecção rápida de packet loss
*/
```

### Reenvio de Pacotes Perdidos

```csharp
public class RetransmissionManager
{
    private readonly Dictionary<EndPoint, Dictionary<uint, PendingPacket>> _pendingPackets = new();
    private readonly Timer _retransmissionTimer;

    public class PendingPacket
    {
        public NetworkPacket Packet { get; set; }
        public DateTime FirstSent { get; set; }
        public DateTime LastSent { get; set; }
        public int RetryCount { get; set; }
        public TimeSpan RTO { get; set; } // Retransmission Timeout
    }

    public RetransmissionManager()
    {
        _retransmissionTimer = new Timer(ProcessRetransmissions, null,
            TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));
    }

    public void AddPendingPacket(EndPoint destination, NetworkPacket packet)
    {
        if (!_pendingPackets.TryGetValue(destination, out var packets))
        {
            packets = new Dictionary<uint, PendingPacket>();
            _pendingPackets[destination] = packets;
        }

        var pendingPacket = new PendingPacket
        {
            Packet = packet,
            FirstSent = DateTime.UtcNow,
            LastSent = DateTime.UtcNow,
            RetryCount = 0,
            RTO = CalculateInitialRTO(destination)
        };

        packets[packet.SequenceNumber] = pendingPacket;
    }

    private void ProcessRetransmissions(object state)
    {
        var now = DateTime.UtcNow;
        var packetsToRetransmit = new List<(EndPoint, PendingPacket)>();

        foreach (var connectionKvp in _pendingPackets)
        {
            var destination = connectionKvp.Key;
            var packets = connectionKvp.Value;

            foreach (var packetKvp in packets.ToList())
            {
                var sequenceNumber = packetKvp.Key;
                var pendingPacket = packetKvp.Value;

                var timeSinceLastSent = now - pendingPacket.LastSent;
                
                if (timeSinceLastSent >= pendingPacket.RTO)
                {
                    pendingPacket.RetryCount++;
                    
                    if (pendingPacket.RetryCount <= MaxRetries)
                    {
                        // Exponential backoff
                        pendingPacket.RTO = TimeSpan.FromMilliseconds(
                            Math.Min(pendingPacket.RTO.TotalMilliseconds * 2, MaxRTO));
                        
                        pendingPacket.LastSent = now;
                        packetsToRetransmit.Add((destination, pendingPacket));
                        
                        // Update metrics
                        NetworkMetrics.PacketsRetransmitted.Increment();
                    }
                    else
                    {
                        // Give up on this packet
                        packets.Remove(sequenceNumber);
                        OnPacketGivenUp?.Invoke(destination, pendingPacket.Packet);
                        
                        NetworkMetrics.PacketsLost.Increment();
                    }
                }
            }
        }

        // Retransmit packets outside of enumeration
        foreach (var (destination, pendingPacket) in packetsToRetransmit)
        {
            SendPacket(destination, pendingPacket.Packet);
        }
    }

    private TimeSpan CalculateInitialRTO(EndPoint destination)
    {
        // Get RTT statistics for this connection
        var rttStats = GetRTTStatistics(destination);
        
        if (rttStats != null)
        {
            // RFC 6298 RTO calculation
            var smoothedRTT = rttStats.SmoothedRTT;
            var rttVariation = rttStats.RTTVariation;
            
            var rto = smoothedRTT + Math.Max(100, 4 * rttVariation);
            return TimeSpan.FromMilliseconds(Math.Max(MinRTO, Math.Min(rto, MaxRTO)));
        }
        
        return TimeSpan.FromMilliseconds(DefaultRTO);
    }

    public void OnPacketAcknowledged(EndPoint sender, uint sequenceNumber)
    {
        if (_pendingPackets.TryGetValue(sender, out var packets))
        {
            if (packets.TryGetValue(sequenceNumber, out var pendingPacket))
            {
                // Calculate RTT for this packet
                var rtt = DateTime.UtcNow - pendingPacket.FirstSent;
                UpdateRTTStatistics(sender, rtt);
                
                // Remove from pending
                packets.Remove(sequenceNumber);
                
                NetworkMetrics.PacketsAcknowledged.Increment();
            }
        }
    }

    private const int MaxRetries = 5;
    private const double MinRTO = 100;    // 100ms minimum
    private const double MaxRTO = 3000;   // 3 second maximum
    private const double DefaultRTO = 500; // 500ms default
}

/*
Por que retransmissão inteligente?

1. Adaptive RTO: Timeout baseado na latência real
2. Exponential Backoff: Evita congestionar rede ruim
3. Selective Retransmission: Apenas pacotes perdidos
4. Give Up Logic: Evita retransmitir indefinidamente
5. RTT Learning: Melhora estimativas ao longo do tempo
*/
```

### Ordenação de Pacotes

```csharp
public class PacketOrderingManager
{
    private readonly Dictionary<uint, OrderingChannel> _channels = new();

    public class OrderingChannel
    {
        public uint NextExpectedSequence { get; set; } = 1;
        public Dictionary<uint, NetworkPacket> BufferedPackets { get; set; } = new();
        public Queue<NetworkPacket> OrderedPackets { get; set; } = new();
        public uint MaxBufferSize { get; set; } = 100;
    }

    public void ProcessPacket(NetworkPacket packet, uint channelId)
    {
        if (!_channels.TryGetValue(channelId, out var channel))
        {
            channel = new OrderingChannel();
            _channels[channelId] = channel;
        }

        var sequenceNumber = packet.SequenceNumber;
        
        if (sequenceNumber == channel.NextExpectedSequence)
        {
            // This is the next packet we're waiting for
            channel.OrderedPackets.Enqueue(packet);
            channel.NextExpectedSequence++;
            
            // Check if we can deliver any buffered packets
            DeliverBufferedPackets(channel);
        }
        else if (sequenceNumber > channel.NextExpectedSequence)
        {
            // Future packet - buffer it
            if (channel.BufferedPackets.Count < channel.MaxBufferSize)
            {
                channel.BufferedPackets[sequenceNumber] = packet;
            }
            else
            {
                // Buffer full - this indicates severe reordering or loss
                Logger.Warning($"Packet ordering buffer full on channel {channelId}");
                
                // Force delivery of oldest buffered packet
                var oldestSequence = channel.BufferedPackets.Keys.Min();
                var oldestPacket = channel.BufferedPackets[oldestSequence];
                channel.BufferedPackets.Remove(oldestSequence);
                
                channel.OrderedPackets.Enqueue(oldestPacket);
                channel.NextExpectedSequence = oldestSequence + 1;
                
                DeliverBufferedPackets(channel);
            }
        }
        else
        {
            // Old packet - likely a duplicate, ignore
            Logger.Debug($"Received old packet {sequenceNumber}, expected {channel.NextExpectedSequence}");
            NetworkMetrics.DuplicatePackets.Increment();
        }
    }

    private void DeliverBufferedPackets(OrderingChannel channel)
    {
        while (channel.BufferedPackets.TryGetValue(channel.NextExpectedSequence, out var packet))
        {
            channel.BufferedPackets.Remove(channel.NextExpectedSequence);
            channel.OrderedPackets.Enqueue(packet);
            channel.NextExpectedSequence++;
        }
    }

    public NetworkPacket? GetNextOrderedPacket(uint channelId)
    {
        if (_channels.TryGetValue(channelId, out var channel) && 
            channel.OrderedPackets.Count > 0)
        {
            return channel.OrderedPackets.Dequeue();
        }
        
        return null;
    }

    public int GetBufferedPacketCount(uint channelId)
    {
        return _channels.TryGetValue(channelId, out var channel) ? 
            channel.BufferedPackets.Count : 0;
    }

    // For ReliableSequenced - only keep latest packet
    public void ProcessSequencedPacket(NetworkPacket packet, uint channelId)
    {
        if (!_channels.TryGetValue(channelId, out var channel))
        {
            channel = new OrderingChannel();
            _channels[channelId] = channel;
        }

        var sequenceNumber = packet.SequenceNumber;
        
        if (sequenceNumber >= channel.NextExpectedSequence)
        {
            // Clear old buffered packets - we only want the latest
            channel.BufferedPackets.Clear();
            channel.OrderedPackets.Clear();
            
            // Deliver this packet immediately
            channel.OrderedPackets.Enqueue(packet);
            channel.NextExpectedSequence = sequenceNumber + 1;
        }
        else
        {
            // Old packet - ignore
            NetworkMetrics.OutOfOrderPackets.Increment();
        }
    }
}

/*
Por que ordenação de pacotes?

1. Application Logic: Muitos sistemas precisam de ordem
2. State Consistency: Evita estados inconsistentes
3. User Experience: Chat em ordem, updates corretos
4. Data Integrity: Operações dependentes executam em ordem
5. Debugging: Mais fácil debuggar com ordem garantida
*/
```

---

## Conclusão do Módulo 3 (Parte 1)

Nesta primeira parte do Módulo 3, estabelecemos os **fundamentos sólidos** do nosso protocolo de rede customizado:

### ✅ O que foi implementado:

#### **3.1 Fundamentos de Redes para Jogos**
- ✅ **Análise TCP vs UDP** com exemplos práticos
- ✅ **Conceitos de Reliable UDP** e implementação base
- ✅ **Medição de latência e jitter** com código funcional
- ✅ **Packet loss tracking** e quality metrics

#### **3.2 Protocolo Base**
- ✅ **Header design otimizado** (24 bytes com todos os campos necessários)
- ✅ **Sistema de handshake** de 3 vias com segurança
- ✅ **Heartbeat e detecção** de conexão automática
- ✅ **Packet types categorizados** para roteamento eficiente

#### **3.3 Sistema de Confiabilidade**
- ✅ **ACK system eficiente** com bitfields para batch acknowledgment
- ✅ **Retransmissão inteligente** com RTO adaptativo
- ✅ **Ordenação de pacotes** com buffering e canais independentes

### 🎯 Características Técnicas Implementadas:

1. **Performance**: Headers compactos, ACKs em batch, retransmissão seletiva
2. **Confiabilidade**: Multiple reliability types, adaptive timeouts
3. **Escalabilidade**: Channel-based ordering, efficient data structures
4. **Observabilidade**: Métricas integradas em todos os componentes
5. **Flexibilidade**: Diferentes tipos de confiabilidade por necessidade

### 🚀 Próxima Parte:

Na **continuação do Módulo 3**, vamos implementar:
- **3.4 Segurança e Criptografia**: AES encryption, key exchange, anti-replay
- **3.5 Compressão e Otimização**: Delta compression, bit packing, bandwidth optimization

**Está pronto para continuar com a parte de segurança e otimização, ou tem alguma dúvida sobre os fundamentos implementados até agora?**

O código criado até aqui já forma uma base sólida para comunicação confiável entre cliente e servidor, com performance otimizada para MMORPGs!