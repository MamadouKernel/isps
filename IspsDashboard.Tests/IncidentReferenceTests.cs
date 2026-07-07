using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Implementations;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace IspsDashboard.Tests;

public class IncidentReferenceTests
{
    [Fact]
    public void NextReference_OnEmptyDb_ShouldReturnFirstOfYear()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var clock = new TestHelpers.FixedClock(new DateTime(2026, 6, 23));
        var audit = new AuditService(db, new HttpContextAccessor());
        var service = new IncidentService(db, clock, audit);

        Assert.Equal("INC-2026-0001", service.NextReference(2026));
    }

    [Fact]
    public async Task NextReference_ShouldIncrementWithinSameYear()
    {
        var db = TestHelpers.CreateInMemoryDb();
        db.Incidents.AddRange(
            new Incident { Reference = "INC-2026-0001", Title = "T1", Description = "D", OccurredAt = DateTime.UtcNow },
            new Incident { Reference = "INC-2026-0004", Title = "T2", Description = "D", OccurredAt = DateTime.UtcNow },
            new Incident { Reference = "INC-2025-0099", Title = "T3", Description = "D", OccurredAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var clock = new TestHelpers.FixedClock(new DateTime(2026, 6, 23));
        var audit = new AuditService(db, new HttpContextAccessor());
        var service = new IncidentService(db, clock, audit);

        Assert.Equal("INC-2026-0005", service.NextReference(2026));
        Assert.Equal("INC-2025-0100", service.NextReference(2025));
        Assert.Equal("INC-2027-0001", service.NextReference(2027));
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistAndGenerateReference()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var clock = new TestHelpers.FixedClock(new DateTime(2026, 6, 23));
        var audit = new AuditService(db, new HttpContextAccessor());
        var service = new IncidentService(db, clock, audit);

        var created = await service.CreateAsync(new IncidentEditViewModel
        {
            Title = "Intrusion détectée",
            Description = "Détection caméra zone B",
            Category = IncidentCategory.IntrusionPerimetre,
            Severity = IncidentSeverity.Majeur,
            OccurredAt = new DateTime(2026, 6, 23, 14, 30, 0)
        }, "user-1");

        Assert.Equal("INC-2026-0001", created.Reference);
        Assert.Equal(IncidentStatus.Ouvert, created.Status);
        Assert.Single(await Task.FromResult(db.Incidents.ToList()));
    }

    [Fact]
    public async Task ChangeStatusAsync_ToClos_ShouldSetClosedAtAndClosedBy()
    {
        var db = TestHelpers.CreateInMemoryDb();
        db.Incidents.Add(new Incident
        {
            Reference = "INC-2026-0001",
            Title = "T",
            Description = "D",
            OccurredAt = DateTime.UtcNow,
            Status = IncidentStatus.EnInvestigation
        });
        await db.SaveChangesAsync();
        var saved = db.Incidents.First();

        var clock = new TestHelpers.FixedClock(new DateTime(2026, 6, 23));
        var audit = new AuditService(db, new HttpContextAccessor());
        var service = new IncidentService(db, clock, audit);

        var ok = await service.ChangeStatusAsync(saved.Id, IncidentStatus.Clos, "user-1");

        Assert.True(ok);
        var updated = db.Incidents.First();
        Assert.Equal(IncidentStatus.Clos, updated.Status);
        Assert.NotNull(updated.ClosedAt);
        Assert.Equal("user-1", updated.ClosedById);
    }
}
