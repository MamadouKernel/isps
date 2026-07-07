using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Quartz;

namespace IspsDashboard.Jobs;

/// <summary>
/// Purge définitivement les entités soft-deleted (corbeille) depuis plus de 90 jours,
/// pour éviter une accumulation indéfinie de lignes IsDeleted=1 en base.
/// </summary>
[DisallowConcurrentExecution]
public class SoftDeletePurgeJob : IJob
{
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(90);

    private readonly ISoftDeleteService _softDelete;
    private readonly ILogger<SoftDeletePurgeJob> _logger;

    public SoftDeletePurgeJob(ISoftDeleteService softDelete, ILogger<SoftDeletePurgeJob> logger)
    {
        _softDelete = softDelete;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var total = 0;
        total += await _softDelete.PurgeOlderThanAsync<AccessPass>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<VehicleAccess>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<Visitor>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<Incident>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<RestrictedZone>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<VesselCall>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<ShiftBriefing>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<Agent>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<Habilitation>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<Camera>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<ExternalContact>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<NonConformity>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<SecurityAudit>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<ExerciseRex>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<SecurityDocument>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<Exercise>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<Checkpoint>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<KpiCard>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<KpiTableRow>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<Gauge>(RetentionPeriod);
        total += await _softDelete.PurgeOlderThanAsync<ProgressBar>(RetentionPeriod);

        if (total > 0)
            _logger.LogInformation("Purge corbeille : {Count} lignes supprimées définitivement (> {Days} j)", total, RetentionPeriod.TotalDays);
    }
}
