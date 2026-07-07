using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IspsDashboard.Tests;

public class PatrolServiceTests
{
    private PatrolService BuildService(out IspsDashboard.Data.ApplicationDbContext db, DateTime now)
    {
        db = TestHelpers.CreateInMemoryDb();
        db.Checkpoints.AddRange(
            new Checkpoint { Id = 1, Code = "CP-01", Label = "Portail", TargetIntervalMinutes = 60, IsActive = true },
            new Checkpoint { Id = 2, Code = "CP-02", Label = "Périmètre", TargetIntervalMinutes = 180, IsActive = true },
            new Checkpoint { Id = 3, Code = "CP-INACTIVE", Label = "Désactivé", TargetIntervalMinutes = 60, IsActive = false });
        db.SaveChanges();
        var clock = new TestHelpers.FixedClock(now);
        var audit = new AuditService(db, new HttpContextAccessor());
        return new PatrolService(db, clock, audit);
    }

    [Fact]
    public async Task RecordScanAsync_PersistsScanWithCheckpoint()
    {
        var service = BuildService(out var db, DateTime.UtcNow);

        var scan = await service.RecordScanAsync("CP-01", "Agent 03", null, "RAS", 5.31, -4.01, null);

        Assert.NotEqual(0, scan.Id);
        Assert.Equal(1, scan.CheckpointId);
        Assert.Equal("Agent 03", scan.AgentLabel);
        Assert.Single(db.PatrolScans);
    }

    [Fact]
    public async Task RecordScanAsync_ThrowsOnUnknownCheckpoint()
    {
        var service = BuildService(out _, DateTime.UtcNow);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RecordScanAsync("UNKNOWN", "X", null, null, null, null, null));
    }

    [Fact]
    public async Task GetOverdueAsync_FlagsCheckpointsBeyondTarget()
    {
        var now = DateTime.UtcNow;
        var service = BuildService(out var db, now);

        // CP-01 scanné il y a 30 min → OK (target 60)
        db.PatrolScans.Add(new PatrolScan { CheckpointId = 1, AgentLabel = "A", ScannedAt = now.AddMinutes(-30) });
        // CP-02 scanné il y a 4 h → en retard (target 180 = 3 h)
        db.PatrolScans.Add(new PatrolScan { CheckpointId = 2, AgentLabel = "A", ScannedAt = now.AddMinutes(-240) });
        await db.SaveChangesAsync();

        var overdue = await service.GetOverdueAsync();

        Assert.Single(overdue);
        Assert.Equal("CP-02", overdue[0].Checkpoint.Code);
    }

    [Fact]
    public async Task GetOverdueAsync_FlagsCheckpointNeverScanned()
    {
        var service = BuildService(out _, DateTime.UtcNow);
        var overdue = await service.GetOverdueAsync();
        Assert.Equal(2, overdue.Count); // CP-01 et CP-02 actifs, jamais scannés
    }
}
