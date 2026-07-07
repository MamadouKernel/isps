using IspsDashboard.Models.Entities;

namespace IspsDashboard.Services.Interfaces;

public interface IPatrolService
{
    Task<IReadOnlyList<Checkpoint>> GetActiveCheckpointsAsync();
    Task<Checkpoint?> GetByCodeAsync(string code);
    Task<PatrolScan> RecordScanAsync(string checkpointCode, string agentLabel, int? agentId,
        string? observations, double? latitude, double? longitude, string? anomalyType);
    Task<IReadOnlyList<PatrolScan>> GetRecentScansAsync(int take = 20);
    Task<IReadOnlyDictionary<int, DateTime?>> GetLastScanByCheckpointAsync();

    /// <summary>Identifie les checkpoints en retard (dernier scan plus vieux que TargetIntervalMinutes).</summary>
    Task<IReadOnlyList<CheckpointOverdue>> GetOverdueAsync();
}

public record CheckpointOverdue(Checkpoint Checkpoint, DateTime? LastScannedAt, int MinutesSinceLast);
