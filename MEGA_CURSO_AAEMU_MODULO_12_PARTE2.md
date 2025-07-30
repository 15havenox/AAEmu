# 📜 **MÓDULO 12 - PARTE 2: SISTEMA DE OBJETIVOS E RECOMPENSAS AVANÇADO**

## 🎯 **CONTINUAÇÃO DO SISTEMA DE OBJETIVOS**

### **3.3 Implementação Completa da Classe QuestObjective**

```csharp
/// <summary>
/// Representa um objetivo específico dentro de uma quest
/// Cada objetivo é uma tarefa individual que o jogador deve completar
/// </summary>
public class QuestObjective
{
    #region Identificação e Descrição
    /// <summary>
    /// ID único do objetivo dentro da quest
    /// Usado para referenciar este objetivo específico
    /// </summary>
    public uint Id { get; set; }
    
    /// <summary>
    /// Texto que aparece no log de quest do jogador
    /// Deve ser claro e mostrar progresso: "Lobos mortos: {current}/{required}"
    /// Suporta placeholders: {current}, {required}, {target_name}
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Descrição mais detalhada para tooltips
    /// Explica exatamente onde encontrar ou como fazer
    /// </summary>
    public string DetailedDescription { get; set; }
    
    /// <summary>
    /// Tipo de objetivo - define como será processado
    /// Cada tipo tem lógica específica de validação
    /// </summary>
    public QuestObjectiveType Type { get; set; }
    
    /// <summary>
    /// Se este objetivo é opcional (não obrigatório para completar a quest)
    /// Útil para objetivos bonus que dão recompensas extras
    /// </summary>
    public bool IsOptional { get; set; } = false;
    
    /// <summary>
    /// Se este objetivo deve ser completado em sequência
    /// (só aparece depois que o anterior for completado)
    /// </summary>
    public bool IsSequential { get; set; } = false;
    
    /// <summary>
    /// ID do objetivo anterior (se IsSequential = true)
    /// Define a ordem de aparição dos objetivos
    /// </summary>
    public uint? PreviousObjectiveId { get; set; }
    #endregion
    
    #region Alvos e Quantidades
    /// <summary>
    /// ID do que deve ser feito (monstro, item, npc, etc.)
    /// Referencia diferentes tabelas dependendo do Type
    /// </summary>
    public uint TargetId { get; set; }
    
    /// <summary>
    /// IDs alternativos que também contam para este objetivo
    /// Exemplo: "Mate qualquer lobo" = [101, 102, 103] (IDs de diferentes lobos)
    /// </summary>
    public List<uint> AlternativeTargetIds { get; set; } = new();
    
    /// <summary>
    /// Quantidade necessária para completar
    /// Para a maioria dos objetivos (matar X, coletar Y)
    /// </summary>
    public int RequiredAmount { get; set; } = 1;
    
    /// <summary>
    /// Quantidade atual do progresso
    /// Atualizada automaticamente pelo sistema
    /// </summary>
    public int CurrentAmount { get; set; } = 0;
    
    /// <summary>
    /// Quantidade mínima individual por "evento"
    /// Exemplo: "Causar pelo menos 1000 de dano" - cada hit deve dar 1000+
    /// </summary>
    public int MinIndividualAmount { get; set; } = 1;
    
    /// <summary>
    /// Se o progresso pode diminuir (ex: manter 10 itens no inventário)
    /// false = progresso só aumenta (padrão)
    /// true = progresso pode subir e descer
    /// </summary>
    public bool CanProgressDecrease { get; set; } = false;
    #endregion
    
    #region Localização e Condições Espaciais
    /// <summary>
    /// Posição específica onde o objetivo deve ser completado
    /// null = qualquer lugar serve
    /// </summary>
    public Vector3? TargetPosition { get; set; }
    
    /// <summary>
    /// Raio de alcance da posição alvo (em metros)
    /// Só usado se TargetPosition não for null
    /// </summary>
    public float TargetRadius { get; set; } = 10f;
    
    /// <summary>
    /// ID da zona onde o objetivo deve ser completado
    /// null = qualquer zona serve
    /// </summary>
    public uint? RequiredZoneId { get; set; }
    
    /// <summary>
    /// Lista de zonas onde o objetivo pode ser completado
    /// Alternativa para permitir múltiplas zonas
    /// </summary>
    public List<uint> AllowedZones { get; set; } = new();
    
    /// <summary>
    /// Altitude mínima para completar o objetivo
    /// Útil para objetivos como "alcançar o topo da montanha"
    /// </summary>
    public float? MinAltitude { get; set; }
    
    /// <summary>
    /// Altitude máxima para completar o objetivo
    /// Útil para objetivos como "explorar as cavernas subterrâneas"
    /// </summary>
    public float? MaxAltitude { get; set; }
    #endregion
    
    #region Condições Temporais
    /// <summary>
    /// Tempo limite para completar este objetivo (em minutos)
    /// null = sem limite de tempo
    /// 0 = deve ser instantâneo
    /// </summary>
    public int? TimeLimit { get; set; }
    
    /// <summary>
    /// Quando o tempo limite começou a contar
    /// Usado internamente pelo sistema
    /// </summary>
    public DateTime? TimeLimitStarted { get; set; }
    
    /// <summary>
    /// Hora específica do dia quando o objetivo pode ser completado
    /// null = qualquer hora serve
    /// Formato: HH:mm (ex: "14:30" = 2:30 PM)
    /// </summary>
    public TimeSpan? RequiredTimeOfDay { get; set; }
    
    /// <summary>
    /// Faixa de horário permitida (início)
    /// Permite objetivos como "entre 18h e 6h" (noite)
    /// </summary>
    public TimeSpan? AllowedTimeStart { get; set; }
    
    /// <summary>
    /// Faixa de horário permitida (fim)
    /// Usado junto com AllowedTimeStart
    /// </summary>
    public TimeSpan? AllowedTimeEnd { get; set; }
    
    /// <summary>
    /// Dias da semana quando o objetivo pode ser completado
    /// Flags enum permite múltiplos dias
    /// </summary>
    public DaysOfWeek AllowedDays { get; set; } = DaysOfWeek.All;
    
    /// <summary>
    /// Se o objetivo só pode ser completado durante determinado clima
    /// null = qualquer clima serve
    /// </summary>
    public WeatherType? RequiredWeather { get; set; }
    #endregion
    
    #region Condições de Estado do Jogador
    /// <summary>
    /// Se o jogador deve estar vivo para completar
    /// false = pode completar mesmo morto (raro, mas útil)
    /// </summary>
    public bool RequirePlayerAlive { get; set; } = true;
    
    /// <summary>
    /// Se o jogador deve estar em combate
    /// null = tanto faz, true = deve estar, false = não pode estar
    /// </summary>
    public bool? RequireInCombat { get; set; }
    
    /// <summary>
    /// HP mínimo do jogador (porcentagem) para completar
    /// null = qualquer HP serve
    /// Exemplo: 0.8f = 80% HP ou mais
    /// </summary>
    public float? MinHealthPercent { get; set; }
    
    /// <summary>
    /// HP máximo do jogador (porcentagem) para completar
    /// Útil para objetivos como "sobreviver com menos de 10% HP"
    /// </summary>
    public float? MaxHealthPercent { get; set; }
    
    /// <summary>
    /// Se o jogador deve estar montado
    /// null = tanto faz, true = deve estar, false = deve estar a pé
    /// </summary>
    public bool? RequireMounted { get; set; }
    
    /// <summary>
    /// Item específico que o jogador deve estar equipado
    /// null = qualquer equipamento serve
    /// </summary>
    public uint? RequiredEquippedItem { get; set; }
    
    /// <summary>
    /// Buff/debuff que o jogador deve ter ativo
    /// null = qualquer estado serve
    /// </summary>
    public uint? RequiredBuff { get; set; }
    
    /// <summary>
    /// Classe que o jogador deve estar usando
    /// null = qualquer classe serve (para quests multi-classe)
    /// </summary>
    public ClassType? RequiredClass { get; set; }
    #endregion
    
    #region Condições de Grupo
    /// <summary>
    /// Se o objetivo requer estar em grupo
    /// null = tanto faz, true = deve estar, false = deve estar solo
    /// </summary>
    public bool? RequireInGroup { get; set; }
    
    /// <summary>
    /// Tamanho mínimo do grupo para este objetivo específico
    /// null = usa configuração da quest pai
    /// </summary>
    public int? MinGroupSizeForObjective { get; set; }
    
    /// <summary>
    /// Se todos do grupo devem estar próximos para contar progresso
    /// Sobrescreve configuração da quest pai para este objetivo
    /// </summary>
    public bool? RequireGroupProximityForObjective { get; set; }
    
    /// <summary>
    /// Se o progresso deve ser compartilhado com o grupo
    /// false = só quem executou a ação ganha progresso
    /// true = todo mundo próximo ganha progresso
    /// </summary>
    public bool ShareProgressWithGroup { get; set; } = true;
    #endregion
    
    #region Condições Especiais
    /// <summary>
    /// Script customizado para validações especiais
    /// Nome do método estático para executar lógica específica
    /// </summary>
    public string CustomValidationScript { get; set; }
    
    /// <summary>
    /// Parâmetros para o script customizado
    /// Permite configurar comportamentos específicos
    /// </summary>
    public Dictionary<string, object> CustomValidationParams { get; set; } = new();
    
    /// <summary>
    /// Se este objetivo pode falhar (e falhar a quest toda)
    /// Exemplo: "Não deixe o NPC morrer"
    /// </summary>
    public bool CanFail { get; set; } = false;
    
    /// <summary>
    /// Condições que fazem este objetivo falhar
    /// Exemplo: NPC específico morre, tempo acaba, etc.
    /// </summary>
    public List<FailCondition> FailConditions { get; set; } = new();
    
    /// <summary>
    /// Se o objetivo deve ser "descoberto" pelo jogador
    /// false = aparece imediatamente na lista
    /// true = só aparece quando certas condições são atendidas
    /// </summary>
    public bool IsHidden { get; set; } = false;
    
    /// <summary>
    /// Condições para revelar um objetivo oculto
    /// </summary>
    public List<RevealCondition> RevealConditions { get; set; } = new();
    #endregion
    
    #region Estado Interno
    /// <summary>
    /// Se está completo
    /// Calculado automaticamente baseado em CurrentAmount >= RequiredAmount
    /// </summary>
    public bool IsCompleted => CurrentAmount >= RequiredAmount;
    
    /// <summary>
    /// Se falhou (para objetivos que podem falhar)
    /// </summary>
    public bool HasFailed { get; set; } = false;
    
    /// <summary>
    /// Quando foi completado
    /// null = ainda não completou
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Quando falhou (se CanFail = true)
    /// null = não falhou
    /// </summary>
    public DateTime? FailedAt { get; set; }
    
    /// <summary>
    /// Se está visível para o jogador atualmente
    /// Usado para objetivos sequenciais ou ocultos
    /// </summary>
    public bool IsVisible { get; set; } = true;
    
    /// <summary>
    /// Progresso em porcentagem (0.0 a 1.0)
    /// </summary>
    public float ProgressPercent => RequiredAmount > 0 ? (float)CurrentAmount / RequiredAmount : 0f;
    #endregion
    
    #region Métodos de Validação
    /// <summary>
    /// Atualiza progresso do objetivo com todas as validações
    /// Este é o método principal chamado pelo sistema
    /// </summary>
    public bool UpdateProgress(Character player, uint eventTargetId, int amount = 1, object additionalData = null)
    {
        // Se já completou ou falhou, não atualiza mais
        if (IsCompleted || HasFailed) return false;
        
        // Se está oculto, não pode ter progresso ainda
        if (IsHidden && !IsVisible) return false;
        
        // Verifica se é o alvo correto
        if (!IsCorrectTarget(eventTargetId)) return false;
        
        // Verifica todas as condições
        if (!ValidateAllConditions(player, additionalData)) return false;
        
        // Verifica condições especiais de falha
        if (CheckFailConditions(player, additionalData))
        {
            MarkAsFailed();
            return false;
        }
        
        // Atualiza o progresso
        var oldAmount = CurrentAmount;
        
        if (CanProgressDecrease)
        {
            CurrentAmount = amount; // Seta valor absoluto
        }
        else
        {
            CurrentAmount = Math.Min(CurrentAmount + amount, RequiredAmount);
        }
        
        // Se completou agora, marca timestamp
        if (!IsCompleted && CurrentAmount >= RequiredAmount)
        {
            CompletedAt = DateTime.Now;
            Logger.Info($"Objective {Id} completed by player {player.Name}");
        }
        
        // Verifica se deve revelar objetivos ocultos
        CheckRevealConditions(player);
        
        return CurrentAmount != oldAmount; // Retorna true se houve mudança
    }
    
    /// <summary>
    /// Verifica se o targetId corresponde a este objetivo
    /// </summary>
    private bool IsCorrectTarget(uint eventTargetId)
    {
        // Alvo principal
        if (eventTargetId == TargetId) return true;
        
        // Alvos alternativos
        if (AlternativeTargetIds.Contains(eventTargetId)) return true;
        
        return false;
    }
    
    /// <summary>
    /// Valida TODAS as condições necessárias para este objetivo
    /// </summary>
    private bool ValidateAllConditions(Character player, object additionalData)
    {
        // Condições de localização
        if (!ValidateLocationConditions(player)) return false;
        
        // Condições temporais
        if (!ValidateTimeConditions()) return false;
        
        // Condições de estado do jogador
        if (!ValidatePlayerStateConditions(player)) return false;
        
        // Condições de grupo
        if (!ValidateGroupConditions(player)) return false;
        
        // Validação customizada
        if (!ValidateCustomConditions(player, additionalData)) return false;
        
        return true;
    }
    
    /// <summary>
    /// Valida condições de localização (zona, posição, altitude)
    /// </summary>
    private bool ValidateLocationConditions(Character player)
    {
        // Zona específica
        if (RequiredZoneId.HasValue && player.ZoneId != RequiredZoneId.Value)
        {
            Logger.Debug($"Player {player.Name} not in required zone {RequiredZoneId} for objective {Id}");
            return false;
        }
        
        // Lista de zonas permitidas
        if (AllowedZones.Any() && !AllowedZones.Contains(player.ZoneId))
        {
            Logger.Debug($"Player {player.Name} not in allowed zones for objective {Id}");
            return false;
        }
        
        // Posição específica
        if (TargetPosition.HasValue)
        {
            var distance = Vector3.Distance(player.Position, TargetPosition.Value);
            if (distance > TargetRadius)
            {
                Logger.Debug($"Player {player.Name} too far from target position for objective {Id} (distance: {distance:F1}m)");
                return false;
            }
        }
        
        // Altitude
        if (MinAltitude.HasValue && player.Position.Z < MinAltitude.Value)
        {
            Logger.Debug($"Player {player.Name} altitude too low for objective {Id}");
            return false;
        }
        
        if (MaxAltitude.HasValue && player.Position.Z > MaxAltitude.Value)
        {
            Logger.Debug($"Player {player.Name} altitude too high for objective {Id}");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Valida condições temporais (hora, dia, clima, limite de tempo)
    /// </summary>
    private bool ValidateTimeConditions()
    {
        var now = DateTime.Now;
        
        // Limite de tempo
        if (TimeLimit.HasValue && TimeLimitStarted.HasValue)
        {
            var elapsed = now - TimeLimitStarted.Value;
            if (elapsed.TotalMinutes > TimeLimit.Value)
            {
                Logger.Debug($"Time limit exceeded for objective {Id}");
                MarkAsFailed();
                return false;
            }
        }
        
        // Hora específica
        if (RequiredTimeOfDay.HasValue)
        {
            if (now.TimeOfDay != RequiredTimeOfDay.Value)
            {
                Logger.Debug($"Not correct time of day for objective {Id}");
                return false;
            }
        }
        
        // Faixa de horário
        if (AllowedTimeStart.HasValue && AllowedTimeEnd.HasValue)
        {
            var currentTime = now.TimeOfDay;
            
            // Faixa normal (ex: 9h às 17h)
            if (AllowedTimeStart.Value < AllowedTimeEnd.Value)
            {
                if (currentTime < AllowedTimeStart.Value || currentTime > AllowedTimeEnd.Value)
                {
                    Logger.Debug($"Outside allowed time range for objective {Id}");
                    return false;
                }
            }
            // Faixa que cruza meia-noite (ex: 22h às 6h)
            else
            {
                if (currentTime < AllowedTimeStart.Value && currentTime > AllowedTimeEnd.Value)
                {
                    Logger.Debug($"Outside allowed time range for objective {Id}");
                    return false;
                }
            }
        }
        
        // Dias da semana
        var currentDay = (DaysOfWeek)(1 << (int)now.DayOfWeek);
        if (!AllowedDays.HasFlag(currentDay))
        {
            Logger.Debug($"Not allowed day of week for objective {Id}");
            return false;
        }
        
        // TODO: Implementar validação de clima quando sistema de clima estiver pronto
        
        return true;
    }
    
    /// <summary>
    /// Valida condições de estado do jogador
    /// </summary>
    private bool ValidatePlayerStateConditions(Character player)
    {
        // Deve estar vivo
        if (RequirePlayerAlive && player.IsDead)
        {
            Logger.Debug($"Player {player.Name} is dead, cannot progress objective {Id}");
            return false;
        }
        
        // Estado de combate
        if (RequireInCombat.HasValue)
        {
            if (RequireInCombat.Value && !player.IsInCombat)
            {
                Logger.Debug($"Player {player.Name} not in combat for objective {Id}");
                return false;
            }
            if (!RequireInCombat.Value && player.IsInCombat)
            {
                Logger.Debug($"Player {player.Name} in combat, cannot progress objective {Id}");
                return false;
            }
        }
        
        // HP mínimo/máximo
        var healthPercent = (float)player.CurrentHealth / player.MaxHealth;
        if (MinHealthPercent.HasValue && healthPercent < MinHealthPercent.Value)
        {
            Logger.Debug($"Player {player.Name} health too low for objective {Id} ({healthPercent:P})");
            return false;
        }
        if (MaxHealthPercent.HasValue && healthPercent > MaxHealthPercent.Value)
        {
            Logger.Debug($"Player {player.Name} health too high for objective {Id} ({healthPercent:P})");
            return false;
        }
        
        // Estado de montaria
        if (RequireMounted.HasValue)
        {
            if (RequireMounted.Value && !player.IsMounted)
            {
                Logger.Debug($"Player {player.Name} not mounted for objective {Id}");
                return false;
            }
            if (!RequireMounted.Value && player.IsMounted)
            {
                Logger.Debug($"Player {player.Name} mounted, cannot progress objective {Id}");
                return false;
            }
        }
        
        // Item equipado
        if (RequiredEquippedItem.HasValue)
        {
            if (!player.Equipment.HasItemEquipped(RequiredEquippedItem.Value))
            {
                Logger.Debug($"Player {player.Name} not wearing required item for objective {Id}");
                return false;
            }
        }
        
        // Buff ativo
        if (RequiredBuff.HasValue)
        {
            if (!player.Buffs.HasBuff(RequiredBuff.Value))
            {
                Logger.Debug($"Player {player.Name} missing required buff for objective {Id}");
                return false;
            }
        }
        
        // Classe
        if (RequiredClass.HasValue && player.Class != RequiredClass.Value)
        {
            Logger.Debug($"Player {player.Name} wrong class for objective {Id}");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Valida condições de grupo
    /// </summary>
    private bool ValidateGroupConditions(Character player)
    {
        // Deve estar em grupo
        if (RequireInGroup.HasValue)
        {
            if (RequireInGroup.Value && player.Group == null)
            {
                Logger.Debug($"Player {player.Name} not in group for objective {Id}");
                return false;
            }
            if (!RequireInGroup.Value && player.Group != null)
            {
                Logger.Debug($"Player {player.Name} in group, cannot progress objective {Id}");
                return false;
            }
        }
        
        // Tamanho mínimo do grupo
        if (MinGroupSizeForObjective.HasValue && player.Group != null)
        {
            if (player.Group.Members.Count < MinGroupSizeForObjective.Value)
            {
                Logger.Debug($"Group too small for objective {Id} ({player.Group.Members.Count} < {MinGroupSizeForObjective})");
                return false;
            }
        }
        
        // Proximidade do grupo (se necessário)
        if (RequireGroupProximityForObjective.HasValue && RequireGroupProximityForObjective.Value && player.Group != null)
        {
            var nearbyMembers = player.Group.Members.Count(member =>
                Vector3.Distance(member.Position, player.Position) <= 50f); // 50m padrão
            
            if (nearbyMembers < player.Group.Members.Count)
            {
                Logger.Debug($"Not all group members nearby for objective {Id}");
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Executa validação customizada se definida
    /// </summary>
    private bool ValidateCustomConditions(Character player, object additionalData)
    {
        if (string.IsNullOrEmpty(CustomValidationScript))
            return true;
        
        try
        {
            // TODO: Implementar sistema de scripts para validações customizadas
            // Por agora, sempre retorna true
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"Error in custom validation for objective {Id}: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Verifica se alguma condição de falha foi ativada
    /// </summary>
    private bool CheckFailConditions(Character player, object additionalData)
    {
        foreach (var failCondition in FailConditions)
        {
            if (failCondition.IsTriggered(player, additionalData))
            {
                Logger.Info($"Fail condition triggered for objective {Id}: {failCondition.Description}");
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Verifica se deve revelar objetivos ocultos
    /// </summary>
    private void CheckRevealConditions(Character player)
    {
        if (!IsHidden || IsVisible) return;
        
        foreach (var revealCondition in RevealConditions)
        {
            if (revealCondition.IsTriggered(player))
            {
                IsVisible = true;
                Logger.Info($"Objective {Id} revealed to player {player.Name}");
                
                // Notifica o cliente sobre novo objetivo
                player.SendPacket(new ObjectiveRevealedPacket(Id));
                break;
            }
        }
    }
    
    /// <summary>
    /// Marca o objetivo como falhado
    /// </summary>
    private void MarkAsFailed()
    {
        HasFailed = true;
        FailedAt = DateTime.Now;
        Logger.Info($"Objective {Id} failed");
    }
    
    /// <summary>
    /// Inicia contagem de tempo limite
    /// </summary>
    public void StartTimeLimit()
    {
        if (TimeLimit.HasValue)
        {
            TimeLimitStarted = DateTime.Now;
            Logger.Debug($"Time limit started for objective {Id}: {TimeLimit} minutes");
        }
    }
    
    /// <summary>
    /// Para contagem de tempo limite
    /// </summary>
    public void StopTimeLimit()
    {
        TimeLimitStarted = null;
        Logger.Debug($"Time limit stopped for objective {Id}");
    }
    
    /// <summary>
    /// Tempo restante em minutos (se houver limite)
    /// </summary>
    public double? GetRemainingTimeMinutes()
    {
        if (!TimeLimit.HasValue || !TimeLimitStarted.HasValue)
            return null;
        
        var elapsed = DateTime.Now - TimeLimitStarted.Value;
        var remaining = TimeLimit.Value - elapsed.TotalMinutes;
        
        return Math.Max(0, remaining);
    }
    
    /// <summary>
    /// Força completar o objetivo (para comandos de admin)
    /// </summary>
    public void ForceComplete()
    {
        CurrentAmount = RequiredAmount;
        CompletedAt = DateTime.Now;
        HasFailed = false;
        FailedAt = null;
        
        Logger.Info($"Objective {Id} force completed by admin");
    }
    
    /// <summary>
    /// Reseta o objetivo para estado inicial
    /// </summary>
    public void Reset()
    {
        CurrentAmount = 0;
        CompletedAt = null;
        HasFailed = false;
        FailedAt = null;
        TimeLimitStarted = null;
        IsVisible = !IsHidden;
        
        Logger.Debug($"Objective {Id} reset to initial state");
    }
    #endregion
    
    #region Métodos de Utilidade
    /// <summary>
    /// Retorna descrição com placeholders substituídos
    /// </summary>
    public string GetFormattedDescription()
    {
        return Description
            .Replace("{current}", CurrentAmount.ToString())
            .Replace("{required}", RequiredAmount.ToString())
            .Replace("{remaining}", (RequiredAmount - CurrentAmount).ToString())
            .Replace("{percent}", $"{ProgressPercent:P0}");
    }
    
    /// <summary>
    /// Retorna informações detalhadas para debug
    /// </summary>
    public string GetDebugInfo()
    {
        var info = new StringBuilder();
        info.AppendLine($"Objective {Id}: {Description}");
        info.AppendLine($"Type: {Type}");
        info.AppendLine($"Progress: {CurrentAmount}/{RequiredAmount} ({ProgressPercent:P})");
        info.AppendLine($"Target: {TargetId}");
        info.AppendLine($"Completed: {IsCompleted}");
        info.AppendLine($"Failed: {HasFailed}");
        info.AppendLine($"Visible: {IsVisible}");
        info.AppendLine($"Optional: {IsOptional}");
        
        if (RequiredZoneId.HasValue)
            info.AppendLine($"Required Zone: {RequiredZoneId}");
        
        if (TimeLimit.HasValue)
        {
            info.AppendLine($"Time Limit: {TimeLimit} minutes");
            var remaining = GetRemainingTimeMinutes();
            if (remaining.HasValue)
                info.AppendLine($"Time Remaining: {remaining:F1} minutes");
        }
        
        return info.ToString();
    }
    
    /// <summary>
    /// Converte para dados que podem ser enviados ao cliente
    /// </summary>
    public QuestObjectiveClientData ToClientData()
    {
        return new QuestObjectiveClientData
        {
            Id = Id,
            Description = GetFormattedDescription(),
            Type = Type,
            CurrentAmount = CurrentAmount,
            RequiredAmount = RequiredAmount,
            IsCompleted = IsCompleted,
            HasFailed = HasFailed,
            IsVisible = IsVisible,
            IsOptional = IsOptional,
            ProgressPercent = ProgressPercent,
            RemainingTimeMinutes = GetRemainingTimeMinutes(),
            TargetPosition = TargetPosition,
            TargetRadius = TargetRadius
        };
    }
    #endregion
}

/// <summary>
/// Todos os tipos possíveis de objetivos no sistema
/// Cada tipo tem lógica específica de processamento
/// </summary>
public enum QuestObjectiveType
{
    // Objetivos de Combate
    KillMonster,        // Matar X monstros específicos
    KillAnyMonster,     // Matar X monstros de qualquer tipo
    KillPlayerPvP,      // Matar X jogadores em PvP
    DamageMonster,      // Causar X dano total a monstros
    DefeatBoss,         // Derrotar boss específico
    SurviveCombat,      // Sobreviver X tempo em combate
    WinDuel,            // Vencer X duelos
    
    // Objetivos de Coleta
    CollectItem,        // Coletar X itens específicos
    CollectAnyItem,     // Coletar X itens de categoria
    HarvestResource,    // Coletar recursos (minerar, pescar, etc.)
    LootItem,           // Conseguir item específico de loot
    FindTreasure,       // Encontrar baús de tesouro
    
    // Objetivos Sociais  
    TalkToNpc,          // Falar com NPC específico
    EscortNpc,          // Escoltar NPC até destino
    ProtectNpc,         // Proteger NPC por tempo
    TalkToPlayer,       // Falar com outro jogador
    JoinGuild,          // Entrar em guilda
    InvitePlayer,       // Convidar jogador para grupo
    
    // Objetivos de Localização
    ReachLocation,      // Chegar em local específico
    ExploreArea,        // Explorar área (descobrir pontos)
    VisitZone,          // Visitar zona específica
    ClimbAltitude,      // Alcançar altitude específica
    
    // Objetivos de Habilidades
    UseSkill,           // Usar habilidade específica
    CastSpell,          // Conjurar magia específica
    UseItem,            // Usar item específico
    EquipItem,          // Equipar item específico
    UnequipItem,        // Desequipar item específico
    
    // Objetivos de Crafting
    CraftItem,          // Craftar item específico
    CraftAnyItem,       // Craftar X itens de categoria
    UpgradeItem,        // Melhorar item específico
    RepairItem,         // Reparar item quebrado
    EnchantItem,        // Encantar item
    SocketGem,          // Engastar gema em item
    
    // Objetivos de Comércio
    SellItem,           // Vender item específico
    BuyItem,            // Comprar item específico
    TradeWithPlayer,    // Trocar itens com jogador
    AuctionItem,        // Leiloar item
    DeliverItem,        // Entregar item para NPC
    
    // Objetivos de Experiência
    GainExperience,     // Ganhar X experiência
    GainLevel,          // Subir X levels
    GainSkillPoints,    // Ganhar pontos de habilidade
    LearnSkill,         // Aprender habilidade específica
    MasterSkill,        // Dominar habilidade (max level)
    
    // Objetivos de Eventos
    ParticipateEvent,   // Participar de evento específico
    WinEvent,           // Vencer evento
    ScorePoints,        // Fazer X pontos em evento
    CompleteRaid,       // Completar raid específica
    CompleteDungeon,    // Completar dungeon específica
    
    // Objetivos de Tempo
    Survive,            // Sobreviver por X tempo
    StayInArea,         // Permanecer em área por X tempo
    WaitForTime,        // Esperar até horário específico
    
    // Objetivos de Estado
    ReachHealthPercent, // Atingir % específica de HP
    ReachManaPercent,   // Atingir % específica de MP
    GetBuff,            // Receber buff específico
    RemoveDebuff,       // Remover debuff específico
    
    // Objetivos Especiais
    TakeScreenshot,     // Tirar screenshot em local
    SendMessage,        // Enviar mensagem específica
    EmoteAction,        // Fazer emote específico
    ActivateObject,     // Ativar objeto do mundo
    OpenContainer,      // Abrir baú/container
    
    // Objetivos Customizados
    CustomScript,       // Executa script personalizado
    CompleteQuest,      // Completar outra quest
    AbandonQuest,       // Abandonar quest específica
    ShareQuest          // Compartilhar quest com jogador
}

/// <summary>
/// Dias da semana (flags para permitir múltiplos)
/// </summary>
[Flags]
public enum DaysOfWeek
{
    None = 0,
    Sunday = 1,
    Monday = 2,
    Tuesday = 4,
    Wednesday = 8,
    Thursday = 16,
    Friday = 32,
    Saturday = 64,
    All = Sunday | Monday | Tuesday | Wednesday | Thursday | Friday | Saturday,
    Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
    Weekend = Saturday | Sunday
}

/// <summary>
/// Tipos de clima para objetivos específicos
/// </summary>
public enum WeatherType
{
    Any,        // Qualquer clima
    Clear,      // Tempo limpo
    Rain,       // Chuva
    Storm,      // Tempestade
    Snow,       // Neve
    Fog,        // Neblina
    Wind        // Vento forte
}

/// <summary>
/// Condição que pode fazer um objetivo falhar
/// </summary>
public class FailCondition
{
    public string Description { get; set; }
    public FailConditionType Type { get; set; }
    public uint TargetId { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    
    public bool IsTriggered(Character player, object additionalData)
    {
        // TODO: Implementar lógica de verificação de condições de falha
        return false;
    }
}

/// <summary>
/// Tipos de condições de falha
/// </summary>
public enum FailConditionType
{
    NpcDies,        // NPC específico morre
    TimeExpires,    // Tempo limite expira
    PlayerDies,     // Jogador morre
    ItemLost,       // Item específico é perdido
    AreaLeft,       // Jogador sai da área
    TargetEscapes   // Alvo foge/escapa
}

/// <summary>
/// Condição para revelar objetivo oculto
/// </summary>
public class RevealCondition
{
    public string Description { get; set; }
    public RevealConditionType Type { get; set; }
    public uint TargetId { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    
    public bool IsTriggered(Character player)
    {
        // TODO: Implementar lógica de verificação de condições de revelação
        return false;
    }
}

/// <summary>
/// Tipos de condições de revelação
/// </summary>
public enum RevealConditionType
{
    ObjectiveCompleted,     // Outro objetivo foi completado
    ItemObtained,          // Item específico foi obtido
    LocationReached,       // Local específico foi alcançado
    TimeElapsed,           // Tempo específico passou
    NpcTalked,             // Falou com NPC específico
    SkillUsed              // Usou habilidade específica
}

/// <summary>
/// Dados de objetivo para envio ao cliente
/// Versão "limpa" sem informações internas
/// </summary>
public class QuestObjectiveClientData
{
    public uint Id { get; set; }
    public string Description { get; set; }
    public QuestObjectiveType Type { get; set; }
    public int CurrentAmount { get; set; }
    public int RequiredAmount { get; set; }
    public bool IsCompleted { get; set; }
    public bool HasFailed { get; set; }
    public bool IsVisible { get; set; }
    public bool IsOptional { get; set; }
    public float ProgressPercent { get; set; }
    public double? RemainingTimeMinutes { get; set; }
    public Vector3? TargetPosition { get; set; }
    public float TargetRadius { get; set; }
}
```

---

## 🏆 **PARTE 4: SISTEMA DE RECOMPENSAS ULTRA AVANÇADO**

### **4.1 A Economia das Recompensas**

As recompensas são a **motivação principal** do jogador. Elas devem ser:

1. **Proporcionais ao esforço**: Quest difícil = recompensa valiosa
2. **Úteis para progressão**: Ajudam o jogador a evoluir
3. **Variadas**: Não só EXP e ouro, mas itens únicos
4. **Balanceadas**: Não quebram a economia do jogo
5. **Satisfatórias**: Geram sensação de conquista

### **4.2 Implementação Completa do Sistema de Recompensas**

```csharp
/// <summary>
/// Representa uma recompensa que o jogador recebe ao completar uma quest
/// Sistema ultra-flexível que suporta qualquer tipo de prêmio
/// </summary>
public class QuestReward
{
    #region Identificação
    /// <summary>
    /// ID único da recompensa (para referência)
    /// </summary>
    public uint Id { get; set; }
    
    /// <summary>
    /// Tipo básico da recompensa
    /// Define como será processada
    /// </summary>
    public QuestRewardType Type { get; set; }
    
    /// <summary>
    /// Subtipo para classificações mais específicas
    /// Ex: Type = Item, SubType = Weapon
    /// </summary>
    public string SubType { get; set; }
    
    /// <summary>
    /// Nome descritivo da recompensa
    /// Para exibição na interface
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Descrição detalhada do que o jogador receberá
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Ícone para exibir na interface
    /// Caminho para o arquivo de imagem
    /// </summary>
    public string IconPath { get; set; }
    #endregion
    
    #region Valores Base
    /// <summary>
    /// ID do item (para recompensas de item)
    /// Referencia a tabela de itens
    /// </summary>
    public uint ItemId { get; set; }
    
    /// <summary>
    /// Quantidade da recompensa
    /// Usado para: quantidade de item, quantidade de ouro, EXP, etc.
    /// </summary>
    public int Amount { get; set; } = 1;
    
    /// <summary>
    /// Experiência base a ser dada
    /// Pode ser modificada por fatores dinâmicos
    /// </summary>
    public uint BaseExperience { get; set; }
    
    /// <summary>
    /// Ouro base a ser dado
    /// Pode ser modificada por fatores dinâmicos
    /// </summary>
    public uint BaseGold { get; set; }
    
    /// <summary>
    /// Pontos de habilidade a serem dados
    /// </summary>
    public uint SkillPoints { get; set; }
    
    /// <summary>
    /// Pontos de reputação com facção específica
    /// </summary>
    public uint ReputationPoints { get; set; }
    
    /// <summary>
    /// ID da facção para reputação
    /// null = não é recompensa de reputação
    /// </summary>
    public uint? FactionId { get; set; }
    #endregion
    
    #region Configurações de Entrega
    /// <summary>
    /// Se a recompensa é obrigatória (sempre dada)
    /// false = recompensa opcional/escolha
    /// </summary>
    public bool IsMandatory { get; set; } = true;
    
    /// <summary>
    /// Se é uma recompensa de escolha (jogador pode selecionar)
    /// Usado para dar opções: escolha entre 3 armas diferentes
    /// </summary>
    public bool IsChoice { get; set; } = false;
    
    /// <summary>
    /// Grupo de escolha (se IsChoice = true)
    /// Recompensas com mesmo grupo são alternativas mutuamente exclusivas
    /// </summary>
    public int ChoiceGroup { get; set; } = 0;
    
    /// <summary>
    /// Se a recompensa é oculta (não aparece na prévia)
    /// Para recompensas surpresa
    /// </summary>
    public bool IsHidden { get; set; } = false;
    
    /// <summary>
    /// Se deve tentar entregar imediatamente ao inventário
    /// false = envia por correio se inventário cheio
    /// </summary>
    public bool ForceImmediateDelivery { get; set; } = false;
    
    /// <summary>
    /// Prioridade de entrega (maior = primeiro)
    /// Para ordenar recompensas na interface
    /// </summary>
    public int Priority { get; set; } = 0;
    #endregion
    
    #region Condições e Restrições
    /// <summary>
    /// Level mínimo do jogador para receber esta recompensa
    /// null = sem restrição
    /// </summary>
    public byte? MinPlayerLevel { get; set; }
    
    /// <summary>
    /// Level máximo do jogador para receber esta recompensa
    /// null = sem restrição
    /// </summary>
    public byte? MaxPlayerLevel { get; set; }
    
    /// <summary>
    /// Classes que podem receber esta recompensa
    /// null = todas as classes
    /// </summary>
    public List<ClassType> AllowedClasses { get; set; } = new();
    
    /// <summary>
    /// Facções que podem receber esta recompensa
    /// null = todas as facções
    /// </summary>
    public List<FactionType> AllowedFactions { get; set; } = new();
    
    /// <summary>
    /// Se requer que o jogador esteja em grupo
    /// null = tanto faz, true = deve estar, false = deve estar solo
    /// </summary>
    public bool? RequireInGroup { get; set; }
    
    /// <summary>
    /// Itens que o jogador NÃO pode ter para receber esta recompensa
    /// Evita dar item duplicado
    /// </summary>
    public List<uint> ForbiddenItems { get; set; } = new();
    
    /// <summary>
    /// Quests que o jogador deve ter completado
    /// Para recompensas especiais baseadas em histórico
    /// </summary>
    public List<uint> RequiredCompletedQuests { get; set; } = new();
    
    /// <summary>
    /// Achievements que o jogador deve ter
    /// Para recompensas especiais de conquistas
    /// </summary>
    public List<uint> RequiredAchievements { get; set; } = new();
    #endregion
    
    #region Modificadores Dinâmicos
    /// <summary>
    /// Multiplicador de experiência baseado no level do jogador
    /// Formula: BaseExperience * GetLevelMultiplier(playerLevel)
    /// </summary>
    public LevelScalingConfig ExperienceScaling { get; set; }
    
    /// <summary>
    /// Multiplicador de ouro baseado no level do jogador
    /// </summary>
    public LevelScalingConfig GoldScaling { get; set; }
    
    /// <summary>
    /// Multiplicador para grupos
    /// Valor > 1.0 = bonus para quem está em grupo
    /// Valor < 1.0 = penalidade para grupos
    /// </summary>
    public float GroupMultiplier { get; set; } = 1.0f;
    
    /// <summary>
    /// Multiplicador para quests diárias
    /// Normalmente menor que 1.0 pois são repetíveis
    /// </summary>
    public float DailyQuestMultiplier { get; set; } = 1.0f;
    
    /// <summary>
    /// Multiplicador para quests de evento
    /// Normalmente maior que 1.0 para incentivar participação
    /// </summary>
    public float EventQuestMultiplier { get; set; } = 1.0f;
    
    /// <summary>
    /// Chance de ser dada (0.0 a 1.0)
    /// Para recompensas aleatórias
    /// 1.0 = sempre dada, 0.5 = 50% chance, etc.
    /// </summary>
    public float ChanceToReceive { get; set; } = 1.0f;
    
    /// <summary>
    /// Se a chance é por jogador (em grupos)
    /// true = cada membro rola individualmente
    /// false = grupo inteiro rola uma vez só
    /// </summary>
    public bool IndividualChance { get; set; } = true;
    #endregion
    
    #region Configurações de Item (se Type = Item)
    /// <summary>
    /// Se o item deve ser binding (vinculado ao jogador)
    /// Sobrescreve configuração padrão do item
    /// </summary>
    public bool? MakeItemBinding { get; set; }
    
    /// <summary>
    /// Quality específica para este item reward
    /// null = usa quality padrão do item
    /// </summary>
    public ItemQuality? OverrideItemQuality { get; set; }
    
    /// <summary>
    /// Enchantment level específico para este item
    /// null = usa padrão do item (normalmente 0)
    /// </summary>
    public byte? ItemEnchantLevel { get; set; }
    
    /// <summary>
    /// Sockets que devem ser criados no item
    /// null = usa padrão do item
    /// </summary>
    public byte? ItemSockets { get; set; }
    
    /// <summary>
    /// Gemas para colocar nos sockets automaticamente
    /// Lista vazia = sockets vazios
    /// </summary>
    public List<uint> ItemGems { get; set; } = new();
    
    /// <summary>
    /// Estatísticas aleatórias para itens com random stats
    /// null = usa sistema padrão de random stats
    /// </summary>
    public ItemRandomStats CustomRandomStats { get; set; }
    
    /// <summary>
    /// Durabilidade específica para o item
    /// null = usa durabilidade máxima padrão
    /// </summary>
    public uint? ItemDurability { get; set; }
    
    /// <summary>
    /// Se o item deve ter nome personalizado
    /// Para itens únicos de quest
    /// </summary>
    public string CustomItemName { get; set; }
    
    /// <summary>
    /// Descrição personalizada para o item
    /// Adiciona lore específica da quest
    /// </summary>
    public string CustomItemDescription { get; set; }
    #endregion
    
    #region Configurações de Título (se Type = Title)
    /// <summary>
    /// ID do título a ser concedido
    /// </summary>
    public uint TitleId { get; set; }
    
    /// <summary>
    /// Se o título deve ser equipado automaticamente
    /// </summary>
    public bool AutoEquipTitle { get; set; } = false;
    
    /// <summary>
    /// Data de expiração do título
    /// null = título permanente
    /// </summary>
    public DateTime? TitleExpirationDate { get; set; }
    #endregion
    
    #region Configurações de Habilidade (se Type = Skill)
    /// <summary>
    /// ID da habilidade a ser ensinada
    /// </summary>
    public uint SkillId { get; set; }
    
    /// <summary>
    /// Level da habilidade a ser ensinada
    /// 0 = level 1 (aprender), >0 = level específico
    /// </summary>
    public byte SkillLevel { get; set; } = 1;
    
    /// <summary>
    /// Se deve sobrescrever level atual (se jogador já tem)
    /// false = só ensina se não tiver ou se for level maior
    /// </summary>
    public bool OverrideExistingSkill { get; set; } = false;
    #endregion
    
    #region Sistema de Entrega
    /// <summary>
    /// Aplica esta recompensa ao jogador
    /// Este é o método principal chamado pelo sistema
    /// </summary>
    public bool GiveToPlayer(Character player, Quest quest = null)
    {
        try
        {
            // Verifica se jogador pode receber
            if (!CanPlayerReceive(player))
            {
                Logger.Warning($"Player {player.Name} cannot receive reward {Id}");
                return false;
            }
            
            // Verifica chance aleatória
            if (!RollChance())
            {
                Logger.Debug($"Player {player.Name} failed chance roll for reward {Id}");
                return false;
            }
            
            // Calcula valores finais com modificadores
            var finalValues = CalculateFinalValues(player, quest);
            
            // Aplica a recompensa baseada no tipo
            switch (Type)
            {
                case QuestRewardType.Experience:
                    GiveExperience(player, finalValues.Experience);
                    break;
                    
                case QuestRewardType.Gold:
                    GiveGold(player, finalValues.Gold);
                    break;
                    
                case QuestRewardType.Item:
                    return GiveItem(player, finalValues);
                    
                case QuestRewardType.SkillPoints:
                    GiveSkillPoints(player, finalValues.SkillPoints);
                    break;
                    
                case QuestRewardType.Reputation:
                    GiveReputation(player, finalValues.Reputation);
                    break;
                    
                case QuestRewardType.Title:
                    GiveTitle(player);
                    break;
                    
                case QuestRewardType.Skill:
                    GiveSkill(player);
                    break;
                    
                case QuestRewardType.Achievement:
                    GiveAchievement(player);
                    break;
                    
                case QuestRewardType.Currency:
                    GiveCurrency(player, finalValues);
                    break;
                    
                case QuestRewardType.Access:
                    GiveAccess(player);
                    break;
                    
                default:
                    Logger.Error($"Unknown reward type: {Type}");
                    return false;
            }
            
            // Log da recompensa
            Logger.Info($"Player {player.Name} received quest reward: {Type} - {Name} (Amount: {finalValues.Amount})");
            
            // Notifica o cliente
            player.SendPacket(new QuestRewardReceivedPacket(this, finalValues));
            
            // Triggers para achievements/statistics
            player.Statistics.RecordQuestReward(Type, finalValues.Amount);
            
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"Error giving reward {Id} to player {player.Name}: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Verifica se o jogador pode receber esta recompensa
    /// </summary>
    private bool CanPlayerReceive(Character player)
    {
        // Level mínimo/máximo
        if (MinPlayerLevel.HasValue && player.Level < MinPlayerLevel.Value)
            return false;
        if (MaxPlayerLevel.HasValue && player.Level > MaxPlayerLevel.Value)
            return false;
        
        // Classe permitida
        if (AllowedClasses.Any() && !AllowedClasses.Contains(player.Class))
            return false;
        
        // Facção permitida
        if (AllowedFactions.Any() && !AllowedFactions.Contains(player.Faction))
            return false;
        
        // Estado de grupo
        if (RequireInGroup.HasValue)
        {
            if (RequireInGroup.Value && player.Group == null)
                return false;
            if (!RequireInGroup.Value && player.Group != null)
                return false;
        }
        
        // Itens proibidos
        foreach (var forbiddenItem in ForbiddenItems)
        {
            if (player.Inventory.HasItem(forbiddenItem))
            {
                Logger.Debug($"Player {player.Name} has forbidden item {forbiddenItem} for reward {Id}");
                return false;
            }
        }
        
        // Quests necessárias
        foreach (var requiredQuest in RequiredCompletedQuests)
        {
            if (!player.CompletedQuests.Contains(requiredQuest))
            {
                Logger.Debug($"Player {player.Name} missing required quest {requiredQuest} for reward {Id}");
                return false;
            }
        }
        
        // Achievements necessários
        foreach (var requiredAchievement in RequiredAchievements)
        {
            if (!player.Achievements.HasAchievement(requiredAchievement))
            {
                Logger.Debug($"Player {player.Name} missing required achievement {requiredAchievement} for reward {Id}");
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Rola chance aleatória de receber a recompensa
    /// </summary>
    private bool RollChance()
    {
        if (ChanceToReceive >= 1.0f) return true;
        if (ChanceToReceive <= 0.0f) return false;
        
        var randomValue = Random.NextDouble();
        return randomValue <= ChanceToReceive;
    }
    
    /// <summary>
    /// Calcula valores finais aplicando todos os modificadores
    /// </summary>
    private RewardCalculationResult CalculateFinalValues(Character player, Quest quest)
    {
        var result = new RewardCalculationResult
        {
            Experience = BaseExperience,
            Gold = BaseGold,
            SkillPoints = SkillPoints,
            Reputation = ReputationPoints,
            Amount = Amount
        };
        
        // Scaling por level
        if (ExperienceScaling != null)
            result.Experience = (uint)(result.Experience * ExperienceScaling.GetMultiplier(player.Level));
        
        if (GoldScaling != null)
            result.Gold = (uint)(result.Gold * GoldScaling.GetMultiplier(player.Level));
        
        // Modificadores de grupo
        if (player.Group != null)
        {
            result.Experience = (uint)(result.Experience * GroupMultiplier);
            result.Gold = (uint)(result.Gold * GroupMultiplier);
            result.SkillPoints = (uint)(result.SkillPoints * GroupMultiplier);
        }
        
        // Modificadores por tipo de quest
        if (quest != null)
        {
            if (quest.IsDaily)
            {
                result.Experience = (uint)(result.Experience * DailyQuestMultiplier);
                result.Gold = (uint)(result.Gold * DailyQuestMultiplier);
            }
            
            if (quest.IsEventQuest)
            {
                result.Experience = (uint)(result.Experience * EventQuestMultiplier);
                result.Gold = (uint)(result.Gold * EventQuestMultiplier);
            }
        }
        
        // Aplica modificadores globais do servidor
        result.Experience = (uint)(result.Experience * ServerConfig.ExperienceRate);
        result.Gold = (uint)(result.Gold * ServerConfig.GoldRate);
        
        return result;
    }
    
    /// <summary>
    /// Dá experiência ao jogador
    /// </summary>
    private void GiveExperience(Character player, uint amount)
    {
        if (amount > 0)
        {
            player.GainExperience(amount);
            Logger.Debug($"Gave {amount} experience to {player.Name}");
        }
    }
    
    /// <summary>
    /// Dá ouro ao jogador
    /// </summary>
    private void GiveGold(Character player, uint amount)
    {
        if (amount > 0)
        {
            player.Currency.AddGold(amount);
            Logger.Debug($"Gave {amount} gold to {player.Name}");
        }
    }
    
    /// <summary>
    /// Dá item ao jogador
    /// </summary>
    private bool GiveItem(Character player, RewardCalculationResult values)
    {
        try
        {
            // Cria o item com configurações especiais
            var item = CreateCustomizedItem();
            if (item == null)
            {
                Logger.Error($"Failed to create item {ItemId} for reward {Id}");
                return false;
            }
            
            // Tenta adicionar ao inventário
            if (player.Inventory.CanAddItem(item, values.Amount))
            {
                player.Inventory.AddItem(item, values.Amount);
                Logger.Debug($"Gave item {item.Name} x{values.Amount} to {player.Name}");
                return true;
            }
            else if (!ForceImmediateDelivery)
            {
                // Se inventário cheio, envia por correio
                MailManager.SendItemByMail(player, item, values.Amount, "Quest Reward", 
                    $"Your inventory was full, so we sent this quest reward by mail.");
                Logger.Debug($"Sent item {item.Name} x{values.Amount} by mail to {player.Name}");
                return true;
            }
            else
            {
                Logger.Warning($"Cannot give item {item.Name} to {player.Name} - inventory full and ForceImmediateDelivery = true");
                return false;
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error giving item reward to {player.Name}: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Cria item customizado com todas as configurações especiais
    /// </summary>
    private Item CreateCustomizedItem()
    {
        var item = ItemManager.CreateItem(ItemId);
        if (item == null) return null;
        
        // Binding
        if (MakeItemBinding.HasValue)
            item.IsBinding = MakeItemBinding.Value;
        
        // Quality
        if (OverrideItemQuality.HasValue)
            item.Quality = OverrideItemQuality.Value;
        
        // Enchantment
        if (ItemEnchantLevel.HasValue)
            item.EnchantLevel = ItemEnchantLevel.Value;
        
        // Sockets
        if (ItemSockets.HasValue)
        {
            item.CreateSockets(ItemSockets.Value);
            
            // Gemas
            for (int i = 0; i < Math.Min(ItemGems.Count, ItemSockets.Value); i++)
            {
                item.InsertGem(i, ItemGems[i]);
            }
        }
        
        // Random stats
        if (CustomRandomStats != null)
            item.ApplyRandomStats(CustomRandomStats);
        
        // Durabilidade
        if (ItemDurability.HasValue)
            item.CurrentDurability = Math.Min(ItemDurability.Value, item.MaxDurability);
        
        // Nome customizado
        if (!string.IsNullOrEmpty(CustomItemName))
            item.CustomName = CustomItemName;
        
        // Descrição customizada
        if (!string.IsNullOrEmpty(CustomItemDescription))
            item.CustomDescription = CustomItemDescription;
        
        return item;
    }
    
    /// <summary>
    /// Dá pontos de habilidade ao jogador
    /// </summary>
    private void GiveSkillPoints(Character player, uint amount)
    {
        if (amount > 0)
        {
            player.SkillPoints += amount;
            Logger.Debug($"Gave {amount} skill points to {player.Name}");
        }
    }
    
    /// <summary>
    /// Dá reputação com facção específica
    /// </summary>
    private void GiveReputation(Character player, uint amount)
    {
        if (amount > 0 && FactionId.HasValue)
        {
            player.Reputation.AddReputation(FactionId.Value, (int)amount);
            Logger.Debug($"Gave {amount} reputation with faction {FactionId} to {player.Name}");
        }
    }
    
    /// <summary>
    /// Concede título ao jogador
    /// </summary>
    private void GiveTitle(Character player)
    {
        if (TitleId > 0)
        {
            player.Titles.UnlockTitle(TitleId, TitleExpirationDate);
            
            if (AutoEquipTitle)
                player.Titles.EquipTitle(TitleId);
                
            Logger.Debug($"Gave title {TitleId} to {player.Name}");
        }
    }
    
    /// <summary>
    /// Ensina habilidade ao jogador
    /// </summary>
    private void GiveSkill(Character player)
    {
        if (SkillId > 0)
        {
            player.Skills.LearnSkill(SkillId, SkillLevel, OverrideExistingSkill);
            Logger.Debug($"Taught skill {SkillId} level {SkillLevel} to {player.Name}");
        }
    }
    
    /// <summary>
    /// Concede achievement ao jogador
    /// </summary>
    private void GiveAchievement(Character player)
    {
        // TODO: Implementar quando sistema de achievements estiver pronto
        Logger.Debug($"Achievement reward not implemented yet");
    }
    
    /// <summary>
    /// Dá moeda alternativa (tokens, pontos especiais, etc.)
    /// </summary>
    private void GiveCurrency(Character player, RewardCalculationResult values)
    {
        // TODO: Implementar sistema de moedas alternativas
        Logger.Debug($"Alternative currency reward not implemented yet");
    }
    
    /// <summary>
    /// Concede acesso especial (áreas, NPCs, funcionalidades)
    /// </summary>
    private void GiveAccess(Character player)
    {
        // TODO: Implementar sistema de acessos especiais
        Logger.Debug($"Access reward not implemented yet");
    }
    #endregion
    
    #region Métodos de Utilidade
    /// <summary>
    /// Retorna preview da recompensa para mostrar ao jogador
    /// </summary>
    public QuestRewardPreview GetPreview(Character player)
    {
        var values = CalculateFinalValues(player, null);
        
        return new QuestRewardPreview
        {
            Type = Type,
            Name = Name,
            Description = Description,
            IconPath = IconPath,
            Amount = values.Amount,
            Experience = values.Experience,
            Gold = values.Gold,
            SkillPoints = values.SkillPoints,
            IsChoice = IsChoice,
            ChancePercent = (int)(ChanceToReceive * 100),
            IsHidden = IsHidden
        };
    }
    
    /// <summary>
    /// Converte para dados que podem ser enviados ao cliente
    /// </summary>
    public QuestRewardClientData ToClientData(Character player)
    {
        var values = CalculateFinalValues(player, null);
        
        return new QuestRewardClientData
        {
            Id = Id,
            Type = Type,
            Name = Name,
            Description = Description,
            IconPath = IconPath,
            Amount = values.Amount,
            Experience = values.Experience,
            Gold = values.Gold,
            SkillPoints = values.SkillPoints,
            ItemId = ItemId,
            IsMandatory = IsMandatory,
            IsChoice = IsChoice,
            ChoiceGroup = ChoiceGroup,
            IsHidden = IsHidden,
            ChancePercent = (int)(ChanceToReceive * 100),
            Priority = Priority
        };
    }
    
    /// <summary>
    /// Retorna informações detalhadas para debug
    /// </summary>
    public string GetDebugInfo()
    {
        var info = new StringBuilder();
        info.AppendLine($"Reward {Id}: {Name}");
        info.AppendLine($"Type: {Type}");
        info.AppendLine($"Amount: {Amount}");
        
        if (BaseExperience > 0) info.AppendLine($"Experience: {BaseExperience}");
        if (BaseGold > 0) info.AppendLine($"Gold: {BaseGold}");
        if (SkillPoints > 0) info.AppendLine($"Skill Points: {SkillPoints}");
        if (ItemId > 0) info.AppendLine($"Item: {ItemId}");
        
        info.AppendLine($"Mandatory: {IsMandatory}");
        info.AppendLine($"Choice: {IsChoice}");
        info.AppendLine($"Hidden: {IsHidden}");
        info.AppendLine($"Chance: {ChanceToReceive:P}");
        
        return info.ToString();
    }
    #endregion
}

/// <summary>
/// Tipos de recompensas disponíveis no sistema
/// </summary>
public enum QuestRewardType
{
    // Recompensas Básicas
    Experience,         // Experiência
    Gold,              // Ouro
    Item,              // Item específico
    SkillPoints,       // Pontos de habilidade
    
    // Recompensas Sociais
    Reputation,        // Reputação com facção
    Title,             // Título para o jogador
    Achievement,       // Conquista/achievement
    
    // Recompensas de Progressão
    Skill,             // Habilidade específica
    Recipe,            // Receita de crafting
    Access,            // Acesso a áreas/funcionalidades
    
    // Moedas Alternativas
    Currency,          // Tokens, pontos especiais
    Honor,             // Pontos de honra (PvP)
    Arena,             // Pontos de arena
    
    // Recompensas Especiais
    Mount,             // Montaria
    Pet,               // Pet/mascote
    Emote,             // Emote/gesture
    Costume,           // Roupa cosmética
    
    // Recompensas de Funcionalidade
    BagSlot,           // Slot extra no inventário
    BankSlot,          // Slot extra no banco
    CharacterSlot,     // Slot extra de personagem
    
    // Recompensas Temporais
    Buff,              // Buff temporário
    XpBoost,           // Boost de experiência
    GoldBoost,         // Boost de ouro
    
    // Recompensas Customizadas
    Script             // Executa script personalizado
}

/// <summary>
/// Configuração de scaling por level
/// </summary>
public class LevelScalingConfig
{
    public float BaseMultiplier { get; set; } = 1.0f;
    public float PerLevelMultiplier { get; set; } = 0.1f;
    public float MinMultiplier { get; set; } = 0.1f;
    public float MaxMultiplier { get; set; } = 5.0f;
    
    public float GetMultiplier(int playerLevel)
    {
        var multiplier = BaseMultiplier + (playerLevel * PerLevelMultiplier);
        return Math.Max(MinMultiplier, Math.Min(MaxMultiplier, multiplier));
    }
}

/// <summary>
/// Resultado do cálculo de valores finais de recompensa
/// </summary>
public class RewardCalculationResult
{
    public uint Experience { get; set; }
    public uint Gold { get; set; }
    public uint SkillPoints { get; set; }
    public uint Reputation { get; set; }
    public int Amount { get; set; }
}

/// <summary>
/// Preview de recompensa para exibir ao jogador
/// </summary>
public class QuestRewardPreview
{
    public QuestRewardType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public int Amount { get; set; }
    public uint Experience { get; set; }
    public uint Gold { get; set; }
    public uint SkillPoints { get; set; }
    public bool IsChoice { get; set; }
    public int ChancePercent { get; set; }
    public bool IsHidden { get; set; }
}

/// <summary>
/// Dados de recompensa para envio ao cliente
/// </summary>
public class QuestRewardClientData
{
    public uint Id { get; set; }
    public QuestRewardType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public int Amount { get; set; }
    public uint Experience { get; set; }
    public uint Gold { get; set; }
    public uint SkillPoints { get; set; }
    public uint ItemId { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsChoice { get; set; }
    public int ChoiceGroup { get; set; }
    public bool IsHidden { get; set; }
    public int ChancePercent { get; set; }
    public int Priority { get; set; }
}

/// <summary>
/// Random stats customizadas para itens
/// </summary>
public class ItemRandomStats
{
    public Dictionary<StatType, int> MinStats { get; set; } = new();
    public Dictionary<StatType, int> MaxStats { get; set; } = new();
    public int NumberOfStats { get; set; } = 3;
}

/// <summary>
/// Tipos de estatísticas para itens
/// </summary>
public enum StatType
{
    Strength,
    Agility,
    Intelligence,
    Stamina,
    Spirit,
    AttackPower,
    SpellPower,
    CriticalChance,
    // ... outras stats
}

/// <summary>
/// Qualidades de item
/// </summary>
public enum ItemQuality
{
    Poor,       // Cinza
    Common,     // Branco
    Uncommon,   // Verde
    Rare,       // Azul
    Epic,       // Roxo
    Legendary,  // Laranja
    Artifact    // Dourado
}
```

---

### **4.3 Sistema de Recompensas de Escolha**

Algumas quests permitem que o jogador **escolha** sua recompensa:

```csharp
/// <summary>
/// Gerencia recompensas de escolha onde jogador seleciona uma opção
/// </summary>
public class QuestChoiceRewardManager
{
    /// <summary>
    /// Processa escolha do jogador entre recompensas alternativas
    /// </summary>
    public static bool ProcessPlayerChoice(Character player, uint questId, uint chosenRewardId)
    {
        var quest = QuestManager.GetQuest(questId);
        if (quest == null) return false;
        
        var activeQuest = player.ActiveQuests.FirstOrDefault(q => q.QuestId == questId);
        if (activeQuest == null) return false;
        
        // Verifica se quest está completa
        if (!quest.IsCompleted()) return false;
        
        // Encontra a recompensa escolhida
        var chosenReward = quest.ChoiceRewards.FirstOrDefault(r => r.Id == chosenRewardId);
        if (chosenReward == null) return false;
        
        // Verifica se jogador pode receber esta recompensa
        if (!chosenReward.CanPlayerReceive(player)) return false;
        
        // Dá todas as recompensas obrigatórias
        foreach (var mandatoryReward in quest.Rewards.Where(r => r.IsMandatory))
        {
            mandatoryReward.GiveToPlayer(player, quest);
        }
        
        // Dá a recompensa escolhida
        chosenReward.GiveToPlayer(player, quest);
        
        // Completa a quest
        QuestManager.CompleteQuest(player, questId);
        
        Logger.Info($"Player {player.Name} completed quest {questId} and chose reward {chosenRewardId}");
        
        return true;
    }
    
    /// <summary>
    /// Retorna recompensas de escolha disponíveis para um jogador
    /// </summary>
    public static List<QuestReward> GetAvailableChoices(Character player, uint questId)
    {
        var quest = QuestManager.GetQuest(questId);
        if (quest == null) return new List<QuestReward>();
        
        return quest.ChoiceRewards
            .Where(reward => reward.CanPlayerReceive(player))
            .OrderBy(reward => reward.Priority)
            .ToList();
    }
}
```

---

## 🎓 **RESUMO DA PARTE 2**

Agora você viu:
- ✅ Sistema de objetivos ultra-avançado com 20+ tipos diferentes
- ✅ Condições complexas de tempo, localização, estado do jogador
- ✅ Sistema de recompensas completamente flexível
- ✅ Recompensas de escolha e aleatórias
- ✅ Scaling dinâmico baseado em level e grupo
- ✅ Sistema robusto de validações e fail-safes

**🎯 PRÓXIMA PARTE:** Quest Manager Avançado e Sistema de Comunicação!