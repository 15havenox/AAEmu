# MÓDULO 2: CONFIGURAÇÃO DO AMBIENTE DE DESENVOLVIMENTO

---

## Introdução ao Módulo 2

Neste módulo, vamos configurar um ambiente de desenvolvimento profissional e robusto para nosso MMORPG. Cada ferramenta foi escolhida por motivos específicos relacionados aos desafios únicos de desenvolvimento de MMORPGs que aprendemos no Módulo 1.

**Por que a configuração correta é crucial para MMORPGs?**

1. **Complexidade do Sistema**: MMORPGs envolvem múltiplos componentes (cliente, vários servidores, databases, ferramentas de monitoramento)
2. **Debugging Distribuído**: Problemas podem ocorrer em qualquer parte da arquitetura distribuída
3. **Performance Critical**: Ferramentas de profiling são essenciais desde o início
4. **Colaboração em Equipe**: Workflows padronizados são fundamentais para equipes grandes
5. **Deployment Complexo**: Ambientes de desenvolvimento devem espelhar produção

---

## 2.1 SETUP DO UNREAL ENGINE 5.6+

### Por que Unreal Engine 5.6+ para MMORPGs?

**Vantagens técnicas específicas para MMORPGs:**

```
1. Networking Robusto
   - Replication system maduro
   - Custom NetDriver support
   - Bandwidth optimization tools
   - Anti-cheat integration points

2. Escalabilidade Visual
   - World Partition para mundos massivos
   - Level streaming automático
   - LOD system avançado
   - Culling otimizado

3. Performance
   - Multithreading nativo
   - Memory pooling
   - Garbage collection otimizado
   - Platform-specific optimizations

4. Ferramentas de Desenvolvimento
   - Blueprint + C++ híbrido
   - Profiling tools integrados
   - Network profiler
   - Memory profiler

5. Ecosystem
   - Asset store massivo
   - Plugins especializados
   - Comunidade ativa
   - Documentação extensa
```

### Instalação do Unreal Engine 5.6+

#### Passo 1: Requisitos do Sistema

**Requisitos Mínimos para Desenvolvimento de MMORPG:**
```
CPU: Intel i7-8700K / AMD Ryzen 7 2700X (8 cores mínimo)
RAM: 32GB (64GB recomendado para projetos grandes)
GPU: RTX 3070 / RX 6700 XT (8GB VRAM mínimo)
Storage: 1TB NVMe SSD (projetos UE5 são grandes)
OS: Windows 10/11 64-bit (versão 1909+)

Por que estes requisitos?
- CPU: Compilation de C++ é CPU-intensive
- RAM: UE5 + Visual Studio + múltiplos servidores
- GPU: Rendering + Lumen + Nanite
- Storage: Assets, builds, cache podem chegar a 100GB+
```

#### Passo 2: Instalação via Epic Games Launcher

**Download e Configuração:**

1. **Baixar Epic Games Launcher**
   ```
   https://www.epicgames.com/store/en-US/download
   
   Por que via Launcher?
   - Updates automáticos
   - Marketplace integration
   - Template downloads
   - Plugin management
   ```

2. **Instalar Unreal Engine 5.6+**
   ```
   Epic Games Launcher → Unreal Engine → Install Engine
   
   Configurações importantes:
   ✅ Install Engine: 5.6.x (latest stable)
   ✅ Target Platforms: Win64
   ✅ Optional Components:
      - Starter Content
      - Templates and Feature Packs
      - Engine Source Code (recomendado para MMORPGs)
   
   Por que Engine Source Code?
   - Debugging profundo
   - Custom networking modifications
   - Performance optimizations
   - Understanding internal systems
   ```

#### Passo 3: Configuração de Desenvolvimento C++

**Instalação do Visual Studio 2022:**

```
Componentes necessários para UE5 C++:

1. Workloads:
   ✅ Desktop development with C++
   ✅ Game development with C++
   ✅ .NET desktop development (para nossos servidores)

2. Individual Components:
   ✅ MSVC v143 - VS 2022 C++ x64/x86 build tools
   ✅ Windows 10/11 SDK (latest version)
   ✅ CMake tools for Visual Studio
   ✅ Git for Windows
   ✅ IntelliCode

3. Extensions (instalar após VS):
   ✅ Visual Assist (autocomplete avançado)
   ✅ UnrealVS (Unreal-specific tools)
   ✅ Resharper C++ (análise de código)

Por que estas ferramentas?
- Visual Assist: Autocomplete superior para UE5
- UnrealVS: Debugging de Blueprint + C++
- Resharper: Code analysis e refactoring
```

**Configuração do Visual Studio para UE5:**

```cpp
// Configurações recomendadas no Visual Studio
// Tools → Options → Text Editor → C/C++ → Advanced

1. IntelliSense:
   - Disable Error Squiggles: True
   - Max Cached Translation Units: 8
   - Fallback Location: $(ProjectDir)

2. Code Analysis:
   - Enable Code Analysis on Build: False (muito lento)
   - Run Code Analysis on Save: False

3. Debugging:
   - Enable Edit and Continue: False (não funciona bem com UE5)
   - Require source files to exactly match: False

Por que estas configurações?
- UE5 tem macros complexas que confundem IntelliSense
- Code Analysis é muito lento em projetos UE5
- Edit and Continue causa problemas com hot reload
```

### Criação do Projeto MMORPG

#### Estrutura de Projeto Recomendada

**Criando o Projeto Base:**

1. **New Project Setup**
   ```
   Epic Games Launcher → Unreal Engine 5.6 → Create Project
   
   Template: Third Person (C++)
   Project Settings:
   - Project Name: MMORPGProject
   - Location: C:\Dev\MMORPG\Client\
   - Blueprint or C++: C++
   - Starter Content: Yes (para prototipagem rápida)
   - Raytracing: Disabled (performance priority)
   - Target Platform: Desktop
   
   Por que Third Person?
   - Base sólida para character movement
   - Camera system já configurado
   - Input bindings básicos
   - Animation Blueprint exemplo
   ```

2. **Configuração Inicial do Projeto**
   ```cpp
   // No Editor: Edit → Project Settings
   
   Engine - General Settings:
   - Default GameMode: MMORPGGameMode (criaremos)
   - Default Pawn Class: MMORPGCharacter (criaremos)
   - Default Player Controller: MMORPGPlayerController (criaremos)
   - Game Instance Class: MMORPGGameInstance (criaremos)
   
   Engine - Network:
   - Max Players: 1000 (para testes locais)
   - Tick Rate: 30 (consistente com servidor)
   - Connection Timeout: 15.0
   - Initial Connect Timeout: 30.0
   
   Por que estas configurações?
   - GameMode: Controla regras do jogo
   - PlayerController: Input e networking
   - GameInstance: Persistência entre levels
   - Network settings: Otimizados para MMORPG
   ```

#### Estrutura de Pastas do Projeto

**Organização Profissional:**

```
MMORPGProject/
├── Content/
│   ├── Core/                    # Sistemas fundamentais
│   │   ├── GameFramework/       # GameMode, PlayerController, etc.
│   │   ├── Network/             # Networking classes
│   │   ├── Data/                # Data Tables, Structs
│   │   └── Interfaces/          # Blueprint Interfaces
│   ├── Characters/              # Player e NPC characters
│   │   ├── Player/
│   │   ├── NPCs/
│   │   └── Animations/
│   ├── World/                   # Level design e environments
│   │   ├── Levels/
│   │   ├── Props/
│   │   └── Materials/
│   ├── UI/                      # Interface de usuário
│   │   ├── HUD/
│   │   ├── Menus/
│   │   └── Widgets/
│   ├── Audio/                   # Som e música
│   ├── VFX/                     # Efeitos visuais
│   └── ThirdParty/              # Assets externos
├── Source/
│   ├── MMORPGProject/           # Código principal
│   │   ├── Public/
│   │   │   ├── Core/            # Headers dos sistemas core
│   │   │   ├── Network/         # Networking headers
│   │   │   ├── Characters/      # Character headers
│   │   │   ├── GameSystems/     # Inventory, Combat, etc.
│   │   │   └── Utils/           # Utilities e helpers
│   │   ├── Private/             # Implementações
│   │   │   ├── Core/
│   │   │   ├── Network/
│   │   │   ├── Characters/
│   │   │   ├── GameSystems/
│   │   │   └── Utils/
│   │   ├── MMORPGProject.Build.cs
│   │   └── MMORPGProject.h
│   └── MMORPGProjectEditor/     # Editor-only code
├── Config/                      # Configurações
├── Plugins/                     # Plugins customizados
└── Documentation/               # Documentação técnica

Por que esta estrutura?
- Separação clara entre sistemas
- Escalável para equipes grandes
- Facilita navegação e manutenção
- Segue convenções da Epic Games
```

#### Configuração de Build System

**MMORPGProject.Build.cs - Configuração Principal:**

```csharp
// Source/MMORPGProject/MMORPGProject.Build.cs
using UnrealBuildTool;

public class MMORPGProject : ModuleRules
{
    public MMORPGProject(ReadOnlyTargetRules Target) : base(Target)
    {
        // Performance settings para MMORPG
        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
        
        // Modules essenciais para MMORPG
        PublicDependencyModuleNames.AddRange(new string[] { 
            "Core", 
            "CoreUObject", 
            "Engine", 
            "InputCore",
            
            // Networking essencial
            "NetCore",
            "NetworkReplayStreaming",
            "Sockets",
            "Networking",
            
            // UI System
            "UMG",
            "Slate",
            "SlateCore",
            
            // Audio
            "AudioCore",
            "AudioMixer",
            
            // Performance
            "RenderCore",
            "RHI"
        });

        PrivateDependencyModuleNames.AddRange(new string[] {
            // HTTP para comunicação com servidores
            "HTTP",
            "Json",
            "JsonUtilities",
            
            // Encryption para segurança
            "CryptoKeys",
            "SSL",
            
            // Development tools
            "UnrealEd",
            "ToolMenus",
            "EditorStyle",
            "EditorWidgets"
        });

        // Configurações de compilação otimizadas
        if (Target.Configuration == UnrealTargetConfiguration.Development ||
            Target.Configuration == UnrealTargetConfiguration.Debug)
        {
            // Debug builds: priorizar tempo de compilação
            PublicDefinitions.Add("MMORPG_DEBUG=1");
            bUseUnity = true;
            MinFilesUsingPrecompiledHeaderOverride = 1;
        }
        else
        {
            // Release builds: priorizar performance
            PublicDefinitions.Add("MMORPG_DEBUG=0");
            bUseUnity = false; // Melhor otimização
            OptimizationLevel = OptimizationMode.Speed;
        }

        // Platform-specific settings
        if (Target.Platform == UnrealTargetPlatform.Win64)
        {
            PublicDefinitions.Add("MMORPG_PLATFORM_WINDOWS=1");
        }

        // MMORPG specific defines
        PublicDefinitions.AddRange(new string[] {
            "MMORPG_MAX_PLAYERS=10000",
            "MMORPG_TICK_RATE=30",
            "MMORPG_ENABLE_ANTICHEAT=1"
        });
    }
}

/*
Por que cada configuração?

1. PCHUsage: Acelera compilação em projetos grandes
2. Networking modules: Essenciais para comunicação UDP customizada
3. HTTP/Json: Comunicação com Auth/Login servers
4. Crypto: Segurança das comunicações
5. Unity builds: Balanceia tempo de compilação vs otimização
6. Platform defines: Código condicional por plataforma
7. MMORPG defines: Constantes específicas do projeto
*/
```

**Target.cs Files - Configuração de Build Targets:**

```csharp
// Source/MMORPGProjectEditor.Target.cs
using UnrealBuildTool;

public class MMORPGProjectEditorTarget : TargetRules
{
    public MMORPGProjectEditorTarget(TargetInfo Target) : base(Target)
    {
        Type = TargetType.Editor;
        DefaultBuildSettings = BuildSettingsVersion.V2;
        ExtraModuleNames.AddRange(new string[] { "MMORPGProject" });
        
        // Editor-specific optimizations
        bUseUnityBuild = true;
        bUsePCHFiles = true;
        
        // Enable hot reload for faster iteration
        bAllowHotReload = true;
        
        // Development tools
        bBuildDeveloperTools = true;
        bBuildWithEditorOnlyData = true;
        
        // Faster linking
        bUseFastMonoCalls = true;
        bUseSharedPCHs = true;
    }
}

// Source/MMORPGProject.Target.cs
using UnrealBuildTool;

public class MMORPGProjectTarget : TargetRules
{
    public MMORPGProjectTarget(TargetInfo Target) : base(Target)
    {
        Type = TargetType.Game;
        DefaultBuildSettings = BuildSettingsVersion.V2;
        ExtraModuleNames.AddRange(new string[] { "MMORPGProject" });
        
        // Performance optimizations for shipping
        if (Configuration == UnrealTargetConfiguration.Shipping)
        {
            bUseUnityBuild = false; // Better optimization
            bUseLTCG = true; // Link-time code generation
            bAllowLTCGInShipping = true;
            
            // Remove development features
            bBuildDeveloperTools = false;
            bBuildWithEditorOnlyData = false;
            
            // Security
            bDisableDebugInfo = true;
            bOmitPCDebugInfoInDevelopment = true;
        }
        
        // MMORPG specific settings
        bWithServerCode = true; // Permite dedicated server builds
        bCompileWithStatsWithoutEngine = true; // Performance stats
        bCompileWithPluginSupport = true; // Plugin system
    }
}

/*
Por que diferentes targets?

Editor Target:
- Otimizado para desenvolvimento
- Hot reload habilitado
- Ferramentas de debug
- Compilação rápida

Game Target:
- Otimizado para performance final
- Sem ferramentas de desenvolvimento
- Máxima otimização em shipping
- Suporte a dedicated server
*/
```

### Configurações Avançadas do Projeto

#### Engine.ini - Configurações de Performance

```ini
; Config/DefaultEngine.ini
[/Script/Engine.Engine]
+ActiveGameNameRedirects=(OldGameName="ThirdPersonBP",NewGameName="/Script/MMORPGProject")
+ActiveGameNameRedirects=(OldGameName="/Script/ThirdPersonBP",NewGameName="/Script/MMORPGProject")

[/Script/Engine.RendererSettings]
; Performance settings para MMORPG
r.Mobile.DisableVertexFog=True
r.Shadow.Virtual.Enable=1
r.VirtualTextures=True
r.VirtualTexturedLightmaps=True

; LOD settings para mundos grandes
r.StaticMeshLODDistanceScale=1.0
r.SkeletalMeshLODBias=0
r.ViewDistanceScale=1.0

; Culling optimizations
r.EarlyZPass=3
r.EarlyZPassOnlyMaterialMasking=1
r.HZBOcclusion=1

[/Script/Engine.NetworkSettings]
; Network settings específicas para MMORPG
n.VerifyPeer=1
net.MaxRepArraySize=2048
net.MaxRepArrayMemory=65536
net.UseAdaptiveNetUpdateFrequency=1
net.NetClientTicksPerSecond=30
net.MaxClientRate=25000
net.MaxInternetClientRate=10000

; Anti-cheat básico
net.AllowEncryption=1
net.EncryptionToken=YourSecretTokenHere

[/Script/Engine.GameNetworkManagerSettings]
; Timeout settings
TotalNetBandwidth=32000
MaxDynamicBandwidth=7000
MinDynamicBandwidth=4000
MoveRepSize=42.0
MAXPOSITIONERRORSQUARED=3.0
MAXNEARZEROVELOCITYSQUARED=9.0
CLIENTADJUSTUPDATECOST=180.0
MAXCLIENTUPDATEINTERVAL=0.25

/*
Por que estas configurações?

Renderer:
- Virtual textures: Reduz uso de VRAM em mundos grandes
- LOD settings: Balanceia qualidade vs performance
- Culling: Remove objetos não visíveis

Network:
- Tick rate 30: Consistente com game server
- Bandwidth limits: Evita saturação
- Encryption: Segurança básica
- Error thresholds: Anti-cheat movement
*/
```

#### Input.ini - Configurações de Input

```ini
; Config/DefaultInput.ini
[/Script/Engine.InputSettings]
-AxisConfig=(AxisKeyName="Gamepad_LeftX",AxisProperties=(DeadZone=0.25,Exponent=1.f,Sensitivity=1.f))
+AxisConfig=(AxisKeyName="Gamepad_LeftX",AxisProperties=(DeadZone=0.25,Exponent=1.f,Sensitivity=1.f))

; MMORPG specific input actions
+ActionMappings=(ActionName="OpenInventory",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=I)
+ActionMappings=(ActionName="OpenCharacterSheet",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=C)
+ActionMappings=(ActionName="OpenGuildPanel",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=G)
+ActionMappings=(ActionName="AutoRun",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=R)
+ActionMappings=(ActionName="ToggleChat",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=Enter)

; Combat actions
+ActionMappings=(ActionName="Attack",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=LeftMouseButton)
+ActionMappings=(ActionName="Block",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=RightMouseButton)
+ActionMappings=(ActionName="Dodge",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=SpaceBar)

; Hotbar actions (1-0 keys)
+ActionMappings=(ActionName="Hotbar1",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=One)
+ActionMappings=(ActionName="Hotbar2",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=Two)
+ActionMappings=(ActionName="Hotbar3",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=Three)
+ActionMappings=(ActionName="Hotbar4",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=Four)
+ActionMappings=(ActionName="Hotbar5",bShift=False,bCtrl=False,bAlt=False,bCmd=False,Key=Five)

; Movement axis
+AxisMappings=(AxisName="MoveForward",Scale=1.000000,Key=W)
+AxisMappings=(AxisName="MoveForward",Scale=-1.000000,Key=S)
+AxisMappings=(AxisName="MoveRight",Scale=-1.000000,Key=A)
+AxisMappings=(AxisName="MoveRight",Scale=1.000000,Key=D)
+AxisMappings=(AxisName="Turn",Scale=1.000000,Key=MouseX)
+AxisMappings=(AxisName="LookUp",Scale=-1.000000,Key=MouseY)

/*
Por que este mapeamento?

1. Padrão MMORPG estabelecido (WASD movement)
2. Hotkeys familiares (I para inventory, C para character)
3. Combat responsivo (mouse buttons)
4. Hotbar acessível (number keys)
5. Chat integrado (Enter key)
*/
```

### Classes Base do Framework

#### GameInstance - Gerenciamento Global

```cpp
// Source/MMORPGProject/Public/Core/MMORPGGameInstance.h
#pragma once

#include "CoreMinimal.h"
#include "Engine/GameInstance.h"
#include "Network/MMORPGNetworkManager.h"
#include "MMORPGGameInstance.generated.h"

/**
 * Game Instance principal do MMORPG
 * Responsável por:
 * - Gerenciar conexões de rede
 * - Persistir dados entre levels
 * - Coordenar sistemas globais
 */
UCLASS()
class MMORPGPROJECT_API UMMORPGGameInstance : public UGameInstance
{
    GENERATED_BODY()

public:
    UMMORPGGameInstance();

    // UGameInstance interface
    virtual void Init() override;
    virtual void Shutdown() override;
    virtual void OnStart() override;

    // Network Management
    UFUNCTION(BlueprintCallable, Category = "MMORPG|Network")
    bool ConnectToAuthServer(const FString& ServerAddress, int32 Port);

    UFUNCTION(BlueprintCallable, Category = "MMORPG|Network")
    bool ConnectToGameServer(const FString& ServerAddress, int32 Port, const FString& SessionToken);

    UFUNCTION(BlueprintCallable, Category = "MMORPG|Network")
    void DisconnectFromServers();

    // Player Data Management
    UFUNCTION(BlueprintCallable, Category = "MMORPG|Player")
    void SavePlayerData();

    UFUNCTION(BlueprintCallable, Category = "MMORPG|Player")
    void LoadPlayerData();

    // Getters
    UFUNCTION(BlueprintPure, Category = "MMORPG|Network")
    class UMMORPGNetworkManager* GetNetworkManager() const { return NetworkManager; }

    UFUNCTION(BlueprintPure, Category = "MMORPG|Player")
    bool IsConnectedToGameServer() const;

protected:
    // Network management
    UPROPERTY()
    class UMMORPGNetworkManager* NetworkManager;

    // Player persistent data
    UPROPERTY(BlueprintReadOnly, Category = "MMORPG|Player")
    FString PlayerUsername;

    UPROPERTY(BlueprintReadOnly, Category = "MMORPG|Player")
    FString AuthToken;

    UPROPERTY(BlueprintReadOnly, Category = "MMORPG|Player")
    FString SessionToken;

    // Configuration
    UPROPERTY(EditDefaultsOnly, Category = "MMORPG|Config")
    FString DefaultAuthServerAddress = TEXT("127.0.0.1");

    UPROPERTY(EditDefaultsOnly, Category = "MMORPG|Config")
    int32 DefaultAuthServerPort = 5001;

private:
    void InitializeNetworking();
    void InitializePlayerData();
    void InitializeGameSystems();
};

/*
Por que GameInstance?

1. Persiste entre levels: Dados não são perdidos ao trocar mapas
2. Singleton pattern: Acesso global aos sistemas
3. Network ownership: Gerencia conexões de longa duração
4. System coordinator: Inicializa e coordena outros sistemas
5. Platform abstraction: Isola diferenças entre plataformas
*/
```

```cpp
// Source/MMORPGProject/Private/Core/MMORPGGameInstance.cpp
#include "Core/MMORPGGameInstance.h"
#include "Network/MMORPGNetworkManager.h"
#include "Engine/Engine.h"
#include "Engine/World.h"

UMMORPGGameInstance::UMMORPGGameInstance()
{
    // Initialize network manager
    NetworkManager = nullptr;
}

void UMMORPGGameInstance::Init()
{
    Super::Init();
    
    UE_LOG(LogTemp, Warning, TEXT("MMORPGGameInstance::Init() - Initializing MMORPG systems"));
    
    // Initialize core systems
    InitializeNetworking();
    InitializePlayerData();
    InitializeGameSystems();
}

void UMMORPGGameInstance::Shutdown()
{
    UE_LOG(LogTemp, Warning, TEXT("MMORPGGameInstance::Shutdown() - Shutting down MMORPG systems"));
    
    // Cleanup network connections
    DisconnectFromServers();
    
    // Save any pending data
    SavePlayerData();
    
    Super::Shutdown();
}

void UMMORPGGameInstance::OnStart()
{
    Super::OnStart();
    
    UE_LOG(LogTemp, Warning, TEXT("MMORPGGameInstance::OnStart() - MMORPG systems started"));
}

bool UMMORPGGameInstance::ConnectToAuthServer(const FString& ServerAddress, int32 Port)
{
    if (!NetworkManager)
    {
        UE_LOG(LogTemp, Error, TEXT("NetworkManager not initialized"));
        return false;
    }
    
    UE_LOG(LogTemp, Warning, TEXT("Connecting to Auth Server: %s:%d"), *ServerAddress, Port);
    
    // Implement actual connection logic
    return NetworkManager->ConnectToAuthServer(ServerAddress, Port);
}

bool UMMORPGGameInstance::ConnectToGameServer(const FString& ServerAddress, int32 Port, const FString& SessionToken)
{
    if (!NetworkManager)
    {
        UE_LOG(LogTemp, Error, TEXT("NetworkManager not initialized"));
        return false;
    }
    
    if (SessionToken.IsEmpty())
    {
        UE_LOG(LogTemp, Error, TEXT("Invalid session token"));
        return false;
    }
    
    this->SessionToken = SessionToken;
    
    UE_LOG(LogTemp, Warning, TEXT("Connecting to Game Server: %s:%d"), *ServerAddress, Port);
    
    return NetworkManager->ConnectToGameServer(ServerAddress, Port, SessionToken);
}

void UMMORPGGameInstance::DisconnectFromServers()
{
    if (NetworkManager)
    {
        NetworkManager->DisconnectAll();
    }
}

bool UMMORPGGameInstance::IsConnectedToGameServer() const
{
    return NetworkManager && NetworkManager->IsConnectedToGameServer();
}

void UMMORPGGameInstance::SavePlayerData()
{
    // TODO: Implement player data saving
    // This will save to local cache and sync with server
    UE_LOG(LogTemp, Warning, TEXT("Saving player data..."));
}

void UMMORPGGameInstance::LoadPlayerData()
{
    // TODO: Implement player data loading
    // This will load from local cache and sync with server
    UE_LOG(LogTemp, Warning, TEXT("Loading player data..."));
}

void UMMORPGGameInstance::InitializeNetworking()
{
    // Create network manager
    NetworkManager = NewObject<UMMORPGNetworkManager>(this);
    
    if (NetworkManager)
    {
        NetworkManager->Initialize();
        UE_LOG(LogTemp, Warning, TEXT("Network Manager initialized successfully"));
    }
    else
    {
        UE_LOG(LogTemp, Error, TEXT("Failed to create Network Manager"));
    }
}

void UMMORPGGameInstance::InitializePlayerData()
{
    // Initialize default player data
    PlayerUsername = TEXT("");
    AuthToken = TEXT("");
    SessionToken = TEXT("");
    
    UE_LOG(LogTemp, Warning, TEXT("Player data initialized"));
}

void UMMORPGGameInstance::InitializeGameSystems()
{
    // Initialize other game systems here
    // Examples: Audio Manager, UI Manager, etc.
    
    UE_LOG(LogTemp, Warning, TEXT("Game systems initialized"));
}

/*
Explicação da implementação:

1. Init(): Chamado quando o GameInstance é criado
   - Inicializa todos os sistemas core
   - Prepara networking
   - Carrega configurações

2. Shutdown(): Chamado quando o jogo está fechando
   - Desconecta de servidores
   - Salva dados pendentes
   - Cleanup de recursos

3. Network methods: Abstraem a complexidade de conexão
   - Validação de parâmetros
   - Logging detalhado
   - Error handling

4. Data persistence: Gerencia dados que persistem entre levels
   - Username, tokens de autenticação
   - Configurações do player
   - Cache de dados do servidor
*/
```

#### PlayerController - Input e Networking

```cpp
// Source/MMORPGProject/Public/Core/MMORPGPlayerController.h
#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PlayerController.h"
#include "MMORPGPlayerController.generated.h"

/**
 * Player Controller principal do MMORPG
 * Responsável por:
 * - Gerenciar input do player
 * - Comunicação cliente-servidor
 * - UI management
 * - Camera control
 */
UCLASS()
class MMORPGPROJECT_API AMMORPGPlayerController : public APlayerController
{
    GENERATED_BODY()

public:
    AMMORPGPlayerController();

    // APawn interface
    virtual void BeginPlay() override;
    virtual void Tick(float DeltaTime) override;
    virtual void SetupInputComponent() override;

    // Network interface
    virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;

    // Input Actions
    UFUNCTION(BlueprintCallable, Category = "MMORPG|Input")
    void OnMoveForward(float Value);

    UFUNCTION(BlueprintCallable, Category = "MMORPG|Input")
    void OnMoveRight(float Value);

    UFUNCTION(BlueprintCallable, Category = "MMORPG|Input")
    void OnTurn(float Value);

    UFUNCTION(BlueprintCallable, Category = "MMORPG|Input")
    void OnLookUp(float Value);

    // UI Actions
    UFUNCTION(BlueprintCallable, Category = "MMORPG|UI")
    void OnOpenInventory();

    UFUNCTION(BlueprintCallable, Category = "MMORPG|UI")
    void OnOpenCharacterSheet();

    UFUNCTION(BlueprintCallable, Category = "MMORPG|UI")
    void OnToggleChat();

    // Combat Actions
    UFUNCTION(BlueprintCallable, Category = "MMORPG|Combat")
    void OnAttack();

    UFUNCTION(BlueprintCallable, Category = "MMORPG|Combat")
    void OnBlock();

    UFUNCTION(BlueprintCallable, Category = "MMORPG|Combat")
    void OnDodge();

    // Hotbar Actions
    UFUNCTION(BlueprintCallable, Category = "MMORPG|Hotbar")
    void OnHotbarSlot(int32 SlotNumber);

    // Network RPCs
    UFUNCTION(Server, Reliable, WithValidation, Category = "MMORPG|Network")
    void ServerMoveCharacter(FVector Direction, float DeltaTime);

    UFUNCTION(Server, Reliable, WithValidation, Category = "MMORPG|Network")
    void ServerPerformAction(const FString& ActionName, const FString& TargetId);

    // UI Management
    UFUNCTION(BlueprintCallable, Category = "MMORPG|UI")
    void ShowHUD();

    UFUNCTION(BlueprintCallable, Category = "MMORPG|UI")
    void HideHUD();

    UFUNCTION(BlueprintCallable, Category = "MMORPG|UI")
    void ToggleUI(const FString& UIName);

protected:
    // Input values
    UPROPERTY(BlueprintReadOnly, Category = "MMORPG|Input")
    float ForwardInput;

    UPROPERTY(BlueprintReadOnly, Category = "MMORPG|Input")
    float RightInput;

    // Network prediction
    UPROPERTY(Replicated, BlueprintReadOnly, Category = "MMORPG|Network")
    FVector ServerPosition;

    UPROPERTY(Replicated, BlueprintReadOnly, Category = "MMORPG|Network")
    FRotator ServerRotation;

    // UI References
    UPROPERTY(BlueprintReadOnly, Category = "MMORPG|UI")
    class UUserWidget* MainHUD;

    UPROPERTY(BlueprintReadOnly, Category = "MMORPG|UI")
    class UUserWidget* InventoryWidget;

    UPROPERTY(BlueprintReadOnly, Category = "MMORPG|UI")
    class UUserWidget* CharacterSheetWidget;

    // UI Classes
    UPROPERTY(EditDefaultsOnly, Category = "MMORPG|UI")
    TSubclassOf<class UUserWidget> MainHUDClass;

    UPROPERTY(EditDefaultsOnly, Category = "MMORPG|UI")
    TSubclassOf<class UUserWidget> InventoryWidgetClass;

    UPROPERTY(EditDefaultsOnly, Category = "MMORPG|UI")
    TSubclassOf<class UUserWidget> CharacterSheetWidgetClass;

private:
    void InitializeUI();
    void InitializeNetworking();
    
    // Input helpers
    void HandleMovementInput();
    void HandleCameraInput();
    
    // Network helpers
    void PredictMovement(float DeltaTime);
    void ReconcilePosition();
};

/*
Por que PlayerController customizado?

1. Input centralization: Todo input passa por aqui
2. Network authority: Valida ações antes de enviar
3. UI coordination: Gerencia todas as interfaces
4. Prediction: Implementa client-side prediction
5. Security: Primeira linha de defesa contra cheating
*/
```

---

## 2.2 SETUP DO BACKEND .NET 8+

### Por que .NET 8+ para Servidores de MMORPG?

**Vantagens técnicas específicas:**

```
1. Performance
   - AOT (Ahead of Time) compilation
   - Minimal APIs para baixa latência
   - System.Text.Json otimizado
   - Span<T> e Memory<T> para zero-allocation

2. Networking
   - System.Net.Sockets otimizado
   - UDP support nativo
   - Async/await pattern maduro
   - HTTP/3 support

3. Scalability
   - Native threading
   - Task parallel library
   - IHostedService para background tasks
   - Built-in dependency injection

4. Ecosystem
   - Entity Framework Core para databases
   - SignalR para real-time communication
   - ASP.NET Core para APIs
   - Extensive NuGet packages

5. Deployment
   - Docker support nativo
   - Kubernetes integration
   - Cloud-native features
   - Cross-platform deployment
```

### Instalação do .NET 8+ SDK

#### Passo 1: Download e Instalação

```bash
# Windows (via PowerShell)
# Download do site oficial: https://dotnet.microsoft.com/download/dotnet/8.0

# Verificar instalação
dotnet --version
# Deve retornar: 8.0.x

# Listar SDKs instalados
dotnet --list-sdks

# Listar runtimes instalados
dotnet --list-runtimes

# Por que .NET 8?
# - LTS (Long Term Support) até 2026
# - Performance improvements significativas
# - Native AOT para startup rápido
# - Minimal APIs para microservices
```

#### Passo 2: Configuração do Ambiente de Desenvolvimento

**Visual Studio 2022 Extensions para .NET:**

```
Extensions essenciais:

1. Productivity:
   ✅ ReSharper (análise de código)
   ✅ CodeMaid (cleanup automático)
   ✅ Productivity Power Tools
   ✅ File Icons (visual organization)

2. Database:
   ✅ SQL Server Data Tools
   ✅ PostgreSQL Tools
   ✅ Redis Desktop Manager integration

3. DevOps:
   ✅ Docker Tools
   ✅ Kubernetes Tools
   ✅ Azure DevOps integration
   ✅ GitHub integration

4. Testing:
   ✅ NUnit Test Adapter
   ✅ xUnit Test Adapter
   ✅ Coverage tools

Por que estas extensions?
- ReSharper: Code analysis e refactoring
- Database tools: Desenvolvimento com múltiplos DBs
- DevOps tools: CI/CD integration
- Testing tools: TDD/BDD support
```

### Estrutura da Solution

#### Criação da Solution Base

```bash
# Criar diretório do projeto
mkdir C:\Dev\MMORPG\Server
cd C:\Dev\MMORPG\Server

# Criar solution
dotnet new sln -n MMORPGServer

# Estrutura de pastas
mkdir src
mkdir tests
mkdir docs
mkdir scripts
mkdir docker

# Por que esta estrutura?
# src/: Código fonte dos projetos
# tests/: Testes unitários e integração
# docs/: Documentação técnica
# scripts/: Scripts de build e deployment
# docker/: Dockerfiles e compose files
```

**Estrutura Completa da Solution:**

```
MMORPGServer/
├── src/
│   ├── MMORPGServer.AuthService/          # Servidor de Autenticação
│   ├── MMORPGServer.LoginService/         # Servidor de Login
│   ├── MMORPGServer.GameService/          # Servidor de Jogo
│   ├── MMORPGServer.ChatService/          # Servidor de Chat
│   ├── MMORPGServer.Common/               # Código compartilhado
│   ├── MMORPGServer.Database/             # Data access layer
│   └── MMORPGServer.Shared/               # DTOs e contratos
├── tests/
│   ├── MMORPGServer.AuthService.Tests/
│   ├── MMORPGServer.LoginService.Tests/
│   ├── MMORPGServer.GameService.Tests/
│   ├── MMORPGServer.ChatService.Tests/
│   ├── MMORPGServer.Common.Tests/
│   └── MMORPGServer.Integration.Tests/
├── docs/
│   ├── api/                               # API documentation
│   ├── architecture/                      # Architecture docs
│   └── deployment/                        # Deployment guides
├── scripts/
│   ├── build.ps1                         # Build script
│   ├── deploy.ps1                        # Deployment script
│   └── test.ps1                          # Test script
├── docker/
│   ├── auth-service/
│   ├── login-service/
│   ├── game-service/
│   ├── chat-service/
│   └── docker-compose.yml
├── .gitignore
├── Directory.Build.props                  # Global MSBuild properties
├── global.json                           # .NET SDK version lock
└── MMORPGServer.sln

Por que esta organização?
- Separação clara de responsabilidades
- Escalável para equipes grandes
- Facilita CI/CD
- Padrão da indústria .NET
```

#### Configuração Global da Solution

**Directory.Build.props - Propriedades Globais:**

```xml
<!-- Directory.Build.props -->
<Project>
  <PropertyGroup>
    <!-- .NET Configuration -->
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>12.0</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <WarningsAsErrors />
    <WarningsNotAsErrors>CS8618;CS8625</WarningsNotAsErrors>

    <!-- Assembly Information -->
    <Company>MMORPG Development Team</Company>
    <Product>MMORPG Server</Product>
    <Copyright>Copyright © 2024</Copyright>
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>

    <!-- Build Configuration -->
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn> <!-- Missing XML comments -->
    
    <!-- Performance -->
    <ServerGarbageCollection>true</ServerGarbageCollection>
    <ConcurrentGarbageCollection>true</ConcurrentGarbageCollection>
    
    <!-- Security -->
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisLevel>latest</AnalysisLevel>
    <CodeAnalysisRuleSet>$(MSBuildThisFileDirectory)CodeAnalysis.ruleset</CodeAnalysisRuleSet>
  </PropertyGroup>

  <!-- Common Package References -->
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
    <PackageReference Include="Serilog.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
  </ItemGroup>

  <!-- Development Dependencies -->
  <ItemGroup Condition="'$(Configuration)' == 'Debug'">
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="8.0.0" />
  </ItemGroup>

  <!-- Production Optimizations -->
  <PropertyGroup Condition="'$(Configuration)' == 'Release'">
    <Optimize>true</Optimize>
    <DebugType>portable</DebugType>
    <DebugSymbols>true</DebugSymbols>
    <PublishTrimmed>true</PublishTrimmed>
    <PublishSingleFile>false</PublishSingleFile> <!-- Para microservices -->
  </PropertyGroup>
</Project>

<!--
Por que estas configurações?

1. Nullable enable: Previne null reference exceptions
2. TreatWarningsAsErrors: Força qualidade de código
3. ServerGC: Otimizado para throughput em servidores
4. EnableNETAnalyzers: Code analysis automático
5. Common packages: Evita duplicação em projetos
6. Release optimizations: Máxima performance em produção
-->
```

**global.json - Lock da Versão SDK:**

```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestMinor"
  },
  "msbuild-sdks": {
    "Microsoft.Build.Traversal": "3.2.0"
  }
}

/*
Por que global.json?

1. Versão consistente: Todos desenvolvedores usam mesmo SDK
2. rollForward: Permite updates de segurança automáticos
3. Build reproducível: CI/CD usa mesma versão
4. Traversal SDK: Facilita builds de solutions grandes
*/
```

### Criação dos Projetos Base

#### 1. Common Library - Código Compartilhado

```bash
# Criar projeto Common
cd src
dotnet new classlib -n MMORPGServer.Common
cd MMORPGServer.Common

# Adicionar à solution
cd ../..
dotnet sln add src/MMORPGServer.Common/MMORPGServer.Common.csproj
```

**MMORPGServer.Common.csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>MMORPGServer.Common</RootNamespace>
    <AssemblyName>MMORPGServer.Common</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <!-- Networking -->
    <PackageReference Include="System.Net.Sockets" Version="4.3.0" />
    <PackageReference Include="System.Memory" Version="4.5.5" />
    
    <!-- Serialization -->
    <PackageReference Include="System.Text.Json" Version="8.0.0" />
    <PackageReference Include="MessagePack" Version="2.5.129" />
    
    <!-- Cryptography -->
    <PackageReference Include="System.Security.Cryptography.Algorithms" Version="4.3.1" />
    
    <!-- Performance -->
    <PackageReference Include="System.Threading.Channels" Version="8.0.0" />
    <PackageReference Include="System.Collections.Immutable" Version="8.0.0" />
    
    <!-- Utilities -->
    <PackageReference Include="CommunityToolkit.HighPerformance" Version="8.2.2" />
  </ItemGroup>

</Project>
```

**Estrutura do Common:**

```csharp
// src/MMORPGServer.Common/Network/PacketTypes.cs
namespace MMORPGServer.Common.Network;

/// <summary>
/// Tipos de pacotes para comunicação UDP customizada
/// </summary>
public enum PacketType : byte
{
    // Connection Management
    Handshake = 0x01,
    HandshakeResponse = 0x02,
    Heartbeat = 0x03,
    Disconnect = 0x04,
    
    // Authentication
    AuthRequest = 0x10,
    AuthResponse = 0x11,
    AuthToken = 0x12,
    
    // Player Actions
    PlayerMove = 0x20,
    PlayerAction = 0x21,
    PlayerChat = 0x22,
    
    // World Updates
    WorldState = 0x30,
    EntityUpdate = 0x31,
    EntitySpawn = 0x32,
    EntityDespawn = 0x33,
    
    // Game Systems
    InventoryUpdate = 0x40,
    CombatAction = 0x41,
    QuestUpdate = 0x42,
    
    // Error Handling
    Error = 0xFF
}

/*
Por que enum byte?
- Apenas 1 byte por pacote
- 256 tipos possíveis
- Performance crítica em networking
- Fácil de estender
*/
```

```csharp
// src/MMORPGServer.Common/Network/NetworkPacket.cs
using System.Buffers;
using System.Text.Json;

namespace MMORPGServer.Common.Network;

/// <summary>
/// Estrutura base para todos os pacotes de rede
/// Otimizada para performance e baixo uso de memória
/// </summary>
public readonly struct NetworkPacket : IDisposable
{
    public PacketType Type { get; }
    public uint SequenceId { get; }
    public uint Timestamp { get; }
    public ReadOnlyMemory<byte> Data { get; }
    
    private readonly IMemoryOwner<byte>? _memoryOwner;

    public NetworkPacket(PacketType type, uint sequenceId, ReadOnlyMemory<byte> data, IMemoryOwner<byte>? memoryOwner = null)
    {
        Type = type;
        SequenceId = sequenceId;
        Timestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Data = data;
        _memoryOwner = memoryOwner;
    }

    /// <summary>
    /// Serializa o pacote para bytes
    /// Usa memory pooling para evitar allocações
    /// </summary>
    public ReadOnlyMemory<byte> Serialize()
    {
        // Header: Type(1) + SequenceId(4) + Timestamp(4) + DataLength(4) = 13 bytes
        const int headerSize = 13;
        var totalSize = headerSize + Data.Length;
        
        var memoryOwner = MemoryPool<byte>.Shared.Rent(totalSize);
        var buffer = memoryOwner.Memory.Span[..totalSize];
        
        var offset = 0;
        
        // Write header
        buffer[offset++] = (byte)Type;
        BitConverter.TryWriteBytes(buffer[offset..], SequenceId);
        offset += 4;
        BitConverter.TryWriteBytes(buffer[offset..], Timestamp);
        offset += 4;
        BitConverter.TryWriteBytes(buffer[offset..], (uint)Data.Length);
        offset += 4;
        
        // Write data
        Data.Span.CopyTo(buffer[offset..]);
        
        return memoryOwner.Memory[..totalSize];
    }

    /// <summary>
    /// Deserializa bytes para pacote
    /// </summary>
    public static NetworkPacket Deserialize(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 13)
            throw new ArgumentException("Buffer too small for packet header");
        
        var offset = 0;
        
        var type = (PacketType)buffer[offset++];
        var sequenceId = BitConverter.ToUInt32(buffer[offset..]);
        offset += 4;
        var timestamp = BitConverter.ToUInt32(buffer[offset..]);
        offset += 4;
        var dataLength = BitConverter.ToUInt32(buffer[offset..]);
        offset += 4;
        
        if (buffer.Length < offset + dataLength)
            throw new ArgumentException("Buffer too small for packet data");
        
        var data = buffer[offset..(offset + (int)dataLength)].ToArray();
        
        return new NetworkPacket(type, sequenceId, data);
    }

    public void Dispose()
    {
        _memoryOwner?.Dispose();
    }
}

/*
Por que esta implementação?

1. readonly struct: Immutable e performance
2. ReadOnlyMemory<byte>: Zero-copy operations
3. IMemoryOwner: Memory pooling para reduzir GC
4. Fixed header size: Parsing rápido
5. IDisposable: Cleanup automático de memória
*/
```

```csharp
// src/MMORPGServer.Common/Utils/PerformanceCounter.cs
using System.Diagnostics;

namespace MMORPGServer.Common.Utils;

/// <summary>
/// Performance counter para métricas de servidor
/// Thread-safe e otimizado para alta frequência
/// </summary>
public class PerformanceCounter
{
    private long _counter;
    private readonly Stopwatch _stopwatch;
    private readonly object _lock = new();

    public PerformanceCounter()
    {
        _stopwatch = Stopwatch.StartNew();
    }

    public void Increment() => Interlocked.Increment(ref _counter);
    public void Add(long value) => Interlocked.Add(ref _counter, value);
    
    public long Count => Interlocked.Read(ref _counter);
    
    public double CountPerSecond
    {
        get
        {
            lock (_lock)
            {
                var elapsed = _stopwatch.Elapsed.TotalSeconds;
                return elapsed > 0 ? Count / elapsed : 0;
            }
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            Interlocked.Exchange(ref _counter, 0);
            _stopwatch.Restart();
        }
    }
}

/*
Por que esta implementação?

1. Interlocked: Thread-safe sem locks
2. Stopwatch: High-precision timing
3. CountPerSecond: Métrica essencial para servidores
4. Reset: Para janelas de tempo específicas
5. Minimal allocations: Performance crítica
*/
```

#### 2. Auth Service - Servidor de Autenticação

```bash
# Criar Auth Service
cd src
dotnet new webapi -n MMORPGServer.AuthService
cd MMORPGServer.AuthService

# Adicionar referências
dotnet add reference ../MMORPGServer.Common/MMORPGServer.Common.csproj

# Adicionar à solution
cd ../..
dotnet sln add src/MMORPGServer.AuthService/MMORPGServer.AuthService.csproj
```

**MMORPGServer.AuthService.csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>MMORPGServer.AuthService</RootNamespace>
    <AssemblyName>MMORPGServer.AuthService</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <!-- Database -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
    
    <!-- Authentication -->
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.3" />
    
    <!-- Security -->
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageReference Include="Microsoft.AspNetCore.RateLimiting" Version="8.0.0" />
    
    <!-- Caching -->
    <PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.0.0" />
    
    <!-- API Documentation -->
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
    
    <!-- Health Checks -->
    <PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../MMORPGServer.Common/MMORPGServer.Common.csproj" />
  </ItemGroup>

</Project>
```

**Program.cs - Configuração do Auth Service:**

```csharp
// src/MMORPGServer.AuthService/Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MMORPGServer.AuthService.Data;
using MMORPGServer.AuthService.Services;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.File("logs/auth-service-.log", rollingInterval: RollingInterval.Day)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", "AuthService");
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Authentication
var jwtKey = builder.Configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AuthPolicy", configure =>
    {
        configure.PermitLimit = 10; // 10 requests
        configure.Window = TimeSpan.FromMinutes(1); // per minute
        configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        configure.QueueLimit = 5;
    });
});

// Custom Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AuthDbContext>()
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

app.Run();

/*
Por que esta configuração?

1. Serilog: Logging estruturado para análise
2. Entity Framework: ORM maduro para SQL Server
3. Redis: Cache distribuído para sessions
4. JWT: Token-based authentication
5. Rate Limiting: Proteção contra ataques
6. Health Checks: Monitoramento de saúde
7. Swagger: Documentação automática da API
*/
```

---

## 2.3 FERRAMENTAS DE DESENVOLVIMENTO

### Controle de Versão com Git

#### Configuração do Repositório

**Estrutura de Branching para MMORPG:**

```bash
# Inicializar repositório
git init
git remote add origin https://github.com/your-org/mmorpg-project.git

# Configurar Git Flow
git flow init

# Estrutura de branches:
# main: Código de produção
# develop: Integração de features
# feature/*: Novas funcionalidades
# release/*: Preparação para release
# hotfix/*: Correções urgentes

Por que Git Flow?
- Organização clara de releases
- Isolamento de features
- Hotfixes sem afetar desenvolvimento
- Padrão da indústria para projetos grandes
```

**.gitignore Completo:**

```gitignore
# Unreal Engine
Binaries/
DerivedDataCache/
Intermediate/
Saved/
*.VC.db
*.opensdf
*.opendb
*.sdf
*.sln.docstates
*.suo
*.pdb
*.user
*.aps
*.pch
*.vspscc
*_i.c
*_p.c
*.ncb
*.tlb
*.tlh
*.bak
*.cache
*.ilk
*.log
[Bb]in
[Dd]ebug*/
*.lib
*.sbr
obj/
[Rr]elease*/
_ReSharper*/
[Tt]est[Rr]esult*
*.vssscc
$tf*/

# .NET
bin/
obj/
*.user
*.suo
*.cache
*.docstates
_ReSharper*/
*.csproj.user
*.build.csdef
*.publish.xml
*.publishproj
packages/
TestResults/
.vs/

# Database
*.mdf
*.ldf
*.ndf

# Logs
logs/
*.log

# Environment files
.env
.env.local
.env.production

# IDE
.vscode/
.idea/
*.swp
*.swo

# OS
.DS_Store
Thumbs.db

# Docker
.dockerignore
```

#### Workflow de Desenvolvimento

**Feature Development Workflow:**

```bash
# 1. Criar nova feature
git checkout develop
git pull origin develop
git flow feature start inventory-system

# 2. Desenvolver feature
# ... fazer mudanças ...
git add .
git commit -m "feat: implement basic inventory system

- Add inventory data structures
- Implement item management
- Add network synchronization
- Add unit tests

Closes #123"

# 3. Finalizar feature
git flow feature finish inventory-system

# 4. Push para review
git push origin develop

Por que este workflow?
- Isolamento de features
- Commits descritivos
- Tracking de issues
- Code review obrigatório
```

**Commit Message Convention:**

```
Formato: <tipo>(<escopo>): <descrição>

<corpo detalhado>

<footer>

Tipos:
- feat: Nova funcionalidade
- fix: Correção de bug
- docs: Documentação
- style: Formatação
- refactor: Refatoração
- test: Testes
- chore: Manutenção

Exemplos:
feat(auth): implement JWT token validation
fix(network): resolve packet loss on high latency
docs(api): add authentication endpoints documentation
test(inventory): add unit tests for item management

Por que convenção?
- Changelogs automáticos
- Semantic versioning
- Facilita code review
- Integração com ferramentas
```

### Docker e Containerização

#### Docker Setup para Desenvolvimento

**docker-compose.dev.yml - Ambiente Local:**

```yaml
# docker/docker-compose.dev.yml
version: '3.8'

services:
  # SQL Server for development
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: mmorpg-sqlserver-dev
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=DevPassword123!
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    networks:
      - mmorpg-network

  # Redis for caching
  redis:
    image: redis:7-alpine
    container_name: mmorpg-redis-dev
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - mmorpg-network

  # PostgreSQL (alternative database)
  postgres:
    image: postgres:15-alpine
    container_name: mmorpg-postgres-dev
    environment:
      - POSTGRES_DB=mmorpg_dev
      - POSTGRES_USER=dev_user
      - POSTGRES_PASSWORD=dev_password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - mmorpg-network

  # Auth Service
  auth-service:
    build:
      context: ../src/MMORPGServer.AuthService
      dockerfile: ../../docker/auth-service/Dockerfile.dev
    container_name: mmorpg-auth-dev
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=MMORPG_Auth_Dev;User Id=sa;Password=DevPassword123!;TrustServerCertificate=true
      - ConnectionStrings__Redis=redis:6379
      - JWT__Key=your-super-secret-jwt-key-here-must-be-at-least-32-characters
      - JWT__Issuer=MMORPG-AuthService
    ports:
      - "5001:80"
    depends_on:
      - sqlserver
      - redis
    networks:
      - mmorpg-network
    volumes:
      - ../src/MMORPGServer.AuthService:/app
      - /app/bin
      - /app/obj

  # Login Service
  login-service:
    build:
      context: ../src/MMORPGServer.LoginService
      dockerfile: ../../docker/login-service/Dockerfile.dev
    container_name: mmorpg-login-dev
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=MMORPG_Login_Dev;User Id=sa;Password=DevPassword123!;TrustServerCertificate=true
      - AuthService__Url=http://auth-service
    ports:
      - "5002:80"
    depends_on:
      - sqlserver
      - auth-service
    networks:
      - mmorpg-network

  # Game Service
  game-service:
    build:
      context: ../src/MMORPGServer.GameService
      dockerfile: ../../docker/game-service/Dockerfile.dev
    container_name: mmorpg-game-dev
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=MMORPG_Game_Dev;User Id=sa;Password=DevPassword123!;TrustServerCertificate=true
      - ConnectionStrings__Redis=redis:6379
    ports:
      - "7777:7777/udp"  # UDP para game server
      - "5003:80"        # HTTP para management
    depends_on:
      - sqlserver
      - redis
    networks:
      - mmorpg-network

  # Chat Service
  chat-service:
    build:
      context: ../src/MMORPGServer.ChatService
      dockerfile: ../../docker/chat-service/Dockerfile.dev
    container_name: mmorpg-chat-dev
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__Redis=redis:6379
    ports:
      - "5004:80"
    depends_on:
      - redis
    networks:
      - mmorpg-network

  # Monitoring - Prometheus
  prometheus:
    image: prom/prometheus:latest
    container_name: mmorpg-prometheus-dev
    ports:
      - "9090:9090"
    volumes:
      - ./monitoring/prometheus.yml:/etc/prometheus/prometheus.yml
    networks:
      - mmorpg-network

  # Monitoring - Grafana
  grafana:
    image: grafana/grafana:latest
    container_name: mmorpg-grafana-dev
    ports:
      - "3000:3000"
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin
    volumes:
      - grafana_data:/var/lib/grafana
    networks:
      - mmorpg-network

volumes:
  sqlserver_data:
  postgres_data:
  redis_data:
  grafana_data:

networks:
  mmorpg-network:
    driver: bridge

# Por que esta configuração?
# 1. Isolamento: Cada serviço em container próprio
# 2. Networking: Comunicação interna via network
# 3. Volumes: Persistência de dados
# 4. Environment: Configuração por ambiente
# 5. Monitoring: Prometheus + Grafana integrados
```

**Dockerfile.dev para Auth Service:**

```dockerfile
# docker/auth-service/Dockerfile.dev
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["MMORPGServer.AuthService.csproj", "./"]
COPY ["../MMORPGServer.Common/MMORPGServer.Common.csproj", "../MMORPGServer.Common/"]
RUN dotnet restore "MMORPGServer.AuthService.csproj"

# Copy source code
COPY . .
COPY ../MMORPGServer.Common ../MMORPGServer.Common

# Build
RUN dotnet build "MMORPGServer.AuthService.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "MMORPGServer.AuthService.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app

# Install debugging tools for development
RUN apt-get update && apt-get install -y \
    curl \
    vim \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "MMORPGServer.AuthService.dll"]

# Por que Dockerfile.dev?
# 1. Debug build: Símbolos de debug incluídos
# 2. Development tools: curl, vim para debugging
# 3. Health check: Monitoramento de saúde
# 4. Multi-stage: Otimização de tamanho
# 5. Base images: Microsoft oficiais
```

#### Scripts de Desenvolvimento

**scripts/dev-setup.ps1:**

```powershell
# scripts/dev-setup.ps1
param(
    [switch]$Clean,
    [switch]$Rebuild
)

Write-Host "🚀 Setting up MMORPG development environment..." -ForegroundColor Green

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Yellow

# Check Docker
if (!(Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker is not installed or not in PATH"
    exit 1
}

# Check Docker Compose
if (!(Get-Command docker-compose -ErrorAction SilentlyContinue)) {
    Write-Error "Docker Compose is not installed or not in PATH"
    exit 1
}

# Check .NET SDK
if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error ".NET SDK is not installed or not in PATH"
    exit 1
}

# Verify .NET version
$dotnetVersion = dotnet --version
if ($dotnetVersion -lt "8.0.0") {
    Write-Error ".NET 8.0 or higher is required. Current version: $dotnetVersion"
    exit 1
}

Write-Host "✅ Prerequisites check passed" -ForegroundColor Green

# Clean if requested
if ($Clean) {
    Write-Host "🧹 Cleaning existing containers and volumes..." -ForegroundColor Yellow
    docker-compose -f docker/docker-compose.dev.yml down -v --remove-orphans
    docker system prune -f
}

# Build .NET projects
Write-Host "🔨 Building .NET projects..." -ForegroundColor Yellow
dotnet restore
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build .NET projects"
    exit 1
}

# Start infrastructure services first
Write-Host "🐳 Starting infrastructure services..." -ForegroundColor Yellow
docker-compose -f docker/docker-compose.dev.yml up -d sqlserver redis postgres

# Wait for databases to be ready
Write-Host "⏳ Waiting for databases to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Run database migrations
Write-Host "📊 Running database migrations..." -ForegroundColor Yellow
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=MMORPG_Auth_Dev;User Id=sa;Password=DevPassword123!;TrustServerCertificate=true"

dotnet ef database update --project src/MMORPGServer.AuthService --startup-project src/MMORPGServer.AuthService

# Start application services
Write-Host "🚀 Starting application services..." -ForegroundColor Yellow
if ($Rebuild) {
    docker-compose -f docker/docker-compose.dev.yml up -d --build
} else {
    docker-compose -f docker/docker-compose.dev.yml up -d
}

# Show status
Write-Host "📊 Service status:" -ForegroundColor Yellow
docker-compose -f docker/docker-compose.dev.yml ps

Write-Host "✅ Development environment is ready!" -ForegroundColor Green
Write-Host ""
Write-Host "Services available at:" -ForegroundColor Cyan
Write-Host "  Auth Service:    http://localhost:5001" -ForegroundColor White
Write-Host "  Login Service:   http://localhost:5002" -ForegroundColor White
Write-Host "  Game Service:    http://localhost:5003" -ForegroundColor White
Write-Host "  Chat Service:    http://localhost:5004" -ForegroundColor White
Write-Host "  Grafana:         http://localhost:3000 (admin/admin)" -ForegroundColor White
Write-Host "  Prometheus:      http://localhost:9090" -ForegroundColor White
Write-Host ""
Write-Host "To stop: docker-compose -f docker/docker-compose.dev.yml down" -ForegroundColor Yellow

# Por que PowerShell script?
# 1. Cross-platform: Funciona no Windows/Linux/Mac
# 2. Error handling: Verifica pré-requisitos
# 3. Automation: Setup completo com um comando
# 4. Flexibility: Parâmetros para diferentes cenários
# 5. Documentation: Output claro do que está acontecendo
```

### CI/CD Pipeline

#### GitHub Actions Workflow

**.github/workflows/ci-cd.yml:**

```yaml
name: MMORPG CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

env:
  DOTNET_VERSION: '8.0.x'
  NODE_VERSION: '18.x'

jobs:
  # Test .NET Backend
  test-backend:
    runs-on: ubuntu-latest
    
    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2022-latest
        env:
          ACCEPT_EULA: Y
          SA_PASSWORD: TestPassword123!
        ports:
          - 1433:1433
        options: >-
          --health-cmd="/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P TestPassword123! -Q 'SELECT 1'"
          --health-interval=10s
          --health-timeout=3s
          --health-retries=3
      
      redis:
        image: redis:7-alpine
        ports:
          - 6379:6379
        options: >-
          --health-cmd="redis-cli ping"
          --health-interval=10s
          --health-timeout=3s
          --health-retries=3

    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Cache NuGet packages
      uses: actions/cache@v3
      with:
        path: ~/.nuget/packages
        key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
        restore-keys: |
          ${{ runner.os }}-nuget-
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Test
      run: dotnet test --no-build --configuration Release --verbosity normal --collect:"XPlat Code Coverage" --results-directory ./coverage
      env:
        ConnectionStrings__DefaultConnection: "Server=localhost;Database=MMORPG_Test;User Id=sa;Password=TestPassword123!;TrustServerCertificate=true"
        ConnectionStrings__Redis: "localhost:6379"
    
    - name: Upload coverage reports
      uses: codecov/codecov-action@v3
      with:
        directory: ./coverage
        fail_ci_if_error: true

  # Test Unreal Engine Client
  test-client:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v1
    
    - name: Cache Unreal Engine
      id: cache-ue
      uses: actions/cache@v3
      with:
        path: C:\UnrealEngine
        key: ${{ runner.os }}-ue-5.6
    
    - name: Download Unreal Engine (if not cached)
      if: steps.cache-ue.outputs.cache-hit != 'true'
      run: |
        # Download UE5 from Epic Games (requires authentication)
        # This is a simplified version - in practice you'd need Epic Games authentication
        Write-Host "Unreal Engine should be pre-installed on the runner or cached"
    
    - name: Generate project files
      run: |
        cd Client/MMORPGProject
        "C:\UnrealEngine\Engine\Binaries\DotNET\UnrealBuildTool.exe" -projectfiles -project="MMORPGProject.uproject" -game -rocket -progress
    
    - name: Build client
      run: |
        cd Client/MMORPGProject
        "C:\UnrealEngine\Engine\Build\BatchFiles\Build.bat" MMORPGProject Win64 Development "MMORPGProject.uproject" -waitmutex

  # Security Scan
  security-scan:
    runs-on: ubuntu-latest
    needs: [test-backend]
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Run Trivy vulnerability scanner
      uses: aquasecurity/trivy-action@master
      with:
        scan-type: 'fs'
        scan-ref: '.'
        format: 'sarif'
        output: 'trivy-results.sarif'
    
    - name: Upload Trivy scan results
      uses: github/codeql-action/upload-sarif@v2
      with:
        sarif_file: 'trivy-results.sarif'

  # Build and Push Docker Images
  build-and-push:
    runs-on: ubuntu-latest
    needs: [test-backend, security-scan]
    if: github.ref == 'refs/heads/main'
    
    strategy:
      matrix:
        service: [auth-service, login-service, game-service, chat-service]
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v3
    
    - name: Log in to Container Registry
      uses: docker/login-action@v3
      with:
        registry: ghcr.io
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Extract metadata
      id: meta
      uses: docker/metadata-action@v5
      with:
        images: ghcr.io/${{ github.repository }}/${{ matrix.service }}
        tags: |
          type=ref,event=branch
          type=ref,event=pr
          type=sha,prefix={{branch}}-
          type=raw,value=latest,enable={{is_default_branch}}
    
    - name: Build and push
      uses: docker/build-push-action@v5
      with:
        context: ./src/MMORPGServer.${{ matrix.service }}
        file: ./docker/${{ matrix.service }}/Dockerfile
        push: true
        tags: ${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}
        cache-from: type=gha
        cache-to: type=gha,mode=max

  # Deploy to Staging
  deploy-staging:
    runs-on: ubuntu-latest
    needs: [build-and-push]
    if: github.ref == 'refs/heads/develop'
    environment: staging
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Deploy to staging
      run: |
        echo "Deploying to staging environment..."
        # Implement staging deployment logic
        # This could use kubectl, helm, or other deployment tools

  # Deploy to Production
  deploy-production:
    runs-on: ubuntu-latest
    needs: [build-and-push]
    if: github.ref == 'refs/heads/main'
    environment: production
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Deploy to production
      run: |
        echo "Deploying to production environment..."
        # Implement production deployment logic
        # This would include blue-green deployment, rollback capability, etc.

# Por que esta pipeline?
# 1. Multi-stage: Testa antes de buildar
# 2. Parallel jobs: Testa backend e frontend simultaneamente
# 3. Security: Vulnerability scanning integrado
# 4. Caching: Acelera builds subsequentes
# 5. Environment gates: Proteção para production
# 6. Docker: Containerização automática
```

---

## Conclusão do Módulo 2

Configuramos um ambiente de desenvolvimento profissional e completo para nosso MMORPG:

### ✅ O que foi configurado:

#### **2.1 Unreal Engine 5.6+ Setup**
- ✅ Instalação completa com Engine Source Code
- ✅ Visual Studio 2022 otimizado para UE5
- ✅ Projeto C++ estruturado profissionalmente
- ✅ Build system configurado para performance
- ✅ Classes base do framework (GameInstance, PlayerController)
- ✅ Configurações de networking e input

#### **2.2 Backend .NET 8+ Setup**
- ✅ .NET 8 SDK com todas as ferramentas
- ✅ Solution estruturada com microserviços
- ✅ Projetos base (Common, AuthService, etc.)
- ✅ Configurações globais otimizadas
- ✅ Database e caching configurados
- ✅ JWT authentication implementado

#### **2.3 Ferramentas de Desenvolvimento**
- ✅ Git workflow profissional com Git Flow
- ✅ Docker environment completo
- ✅ Scripts de automação
- ✅ CI/CD pipeline com GitHub Actions
- ✅ Monitoring com Prometheus/Grafana
- ✅ Security scanning integrado

### 🎯 Por que este setup é crucial para MMORPGs:

1. **Complexidade Gerenciada**: Ferramentas adequadas para sistemas distribuídos
2. **Performance desde o início**: Configurações otimizadas para alta carga
3. **Collaboration Ready**: Workflows para equipes grandes
4. **Production Ready**: Ambiente que espelha produção
5. **Monitoring Integrado**: Observabilidade desde desenvolvimento

### 🚀 Próximos Passos:

No **Módulo 3**, vamos implementar nosso **Protocolo de Rede Customizado sobre UDP**:

1. **Fundamentos de Networking** para jogos
2. **Protocolo UDP confiável** com handshake e ACK
3. **Sistema de criptografia** para segurança
4. **Compressão e otimização** de pacotes
5. **Implementação em C#** para servidores
6. **Implementação em C++** para Unreal Engine

**Está pronto para mergulhar no networking avançado, ou tem alguma dúvida sobre o ambiente de desenvolvimento?**

Todo o código criado neste módulo será a base sólida para implementarmos os sistemas complexos dos próximos módulos. É importante que o ambiente esteja funcionando perfeitamente antes de avançarmos para a implementação do protocolo de rede!