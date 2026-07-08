using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class PatrolService : IPatrolService
{
    private readonly ApplicationDbContext _db;
    private readonly IClock _clock;
    private readonly IAuditService _audit;

    public PatrolService(ApplicationDbContext db, IClock clock, IAuditService audit)
    {
        _db = db;
        _clock = clock;
        _audit = audit;
    }

    public async Task<IReadOnlyList<Checkpoint>> GetActiveCheckpointsAsync()
        => await _db.Checkpoints.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Code)
            .ToListAsync();

    public Task<Checkpoint?> GetByCodeAsync(string code)
        => _db.Checkpoints.FirstOrDefaultAsync(c => c.Code == code && c.IsActive);

    public async Task<Checkpoint> CreateCheckpointAsync(string code, string label, string? zone, int targetIntervalMinutes)
    {
        var entity = new Checkpoint
        {
            Code = code.Trim(),
            Label = label.Trim(),
            Zone = zone?.Trim() ?? string.Empty,
            TargetIntervalMinutes = targetIntervalMinutes > 0 ? targetIntervalMinutes : 240,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.Checkpoints.Add(entity);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AddCheckpoint", $"{entity.Code} — {entity.Label}");
        return entity;
    }

    public async Task<PatrolScan> RecordScanAsync(
        string checkpointCode, string agentLabel, int? agentId,
        string? observations, double? latitude, double? longitude, string? anomalyType)
    {
        var checkpoint = await GetByCodeAsync(checkpointCode)
            ?? throw new InvalidOperationException($"Checkpoint inconnu : {checkpointCode}");

        var scan = new PatrolScan
        {
            CheckpointId = checkpoint.Id,
            AgentId = agentId,
            AgentLabel = string.IsNullOrWhiteSpace(agentLabel) ? "anonyme" : agentLabel.Trim(),
            ScannedAt = DateTime.UtcNow,
            Observations = observations?.Trim(),
            Latitude = latitude,
            Longitude = longitude,
            AnomalyType = string.IsNullOrWhiteSpace(anomalyType) ? null : anomalyType.Trim()
        };
        _db.PatrolScans.Add(scan);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("PatrolScan", $"{checkpoint.Code} par {scan.AgentLabel}" +
            (scan.AnomalyType is null ? "" : $" — ANOMALIE: {scan.AnomalyType}"));
        return scan;
    }

    public async Task<IReadOnlyList<PatrolScan>> GetRecentScansAsync(int take = 20)
        => await _db.PatrolScans.AsNoTracking()
            .Include(p => p.Checkpoint)
            .OrderByDescending(p => p.ScannedAt)
            .Take(take)
            .ToListAsync();

    public async Task<IReadOnlyDictionary<int, DateTime?>> GetLastScanByCheckpointAsync()
    {
        var data = await _db.PatrolScans.AsNoTracking()
            .GroupBy(p => p.CheckpointId)
            .Select(g => new { CheckpointId = g.Key, LastAt = (DateTime?)g.Max(p => p.ScannedAt) })
            .ToListAsync();
        return data.ToDictionary(x => x.CheckpointId, x => x.LastAt);
    }

    public async Task<IReadOnlyList<CheckpointOverdue>> GetOverdueAsync()
    {
        var checkpoints = await GetActiveCheckpointsAsync();
        var lastScans = await GetLastScanByCheckpointAsync();
        var now = DateTime.UtcNow;

        return checkpoints
            .Select(c =>
            {
                lastScans.TryGetValue(c.Id, out var last);
                var minutes = last is null ? int.MaxValue : (int)(now - last.Value).TotalMinutes;
                return new CheckpointOverdue(c, last, minutes);
            })
            .Where(o => o.MinutesSinceLast > o.Checkpoint.TargetIntervalMinutes)
            .OrderByDescending(o => o.MinutesSinceLast)
            .ToList();
    }
}
