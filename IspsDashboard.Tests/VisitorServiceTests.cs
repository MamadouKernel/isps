using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IspsDashboard.Tests;

public class VisitorServiceTests
{
    private VisitorService BuildService(out IspsDashboard.Data.ApplicationDbContext db, DateTime today)
    {
        db = TestHelpers.CreateInMemoryDb();
        var clock = new TestHelpers.FixedClock(today);
        var audit = new AuditService(db, new HttpContextAccessor());
        return new VisitorService(db, clock, audit);
    }

    [Fact]
    public async Task CreateAsync_GeneratesReferenceAndDefaultsToPreEnregistre()
    {
        var service = BuildService(out _, new DateTime(2026, 6, 23));

        var v = await service.CreateAsync(new Visitor
        {
            FullName = "John Doe",
            Purpose = "Inspection",
            ScheduledArrival = new DateTime(2026, 6, 23, 10, 0, 0)
        });

        Assert.Equal("VIS-2026-0001", v.Reference);
        Assert.Equal(VisitorStatus.PreEnregistre, v.Status);
    }

    [Fact]
    public async Task CheckIn_SetsStatusAndTimestamp()
    {
        var service = BuildService(out var db, new DateTime(2026, 6, 23));
        var v = await service.CreateAsync(new Visitor { FullName = "X", Purpose = "p", ScheduledArrival = DateTime.UtcNow });

        var ok = await service.CheckInAsync(v.Id, "PFSO", "B-099");

        Assert.True(ok);
        var reloaded = db.Visitors.First();
        Assert.Equal(VisitorStatus.SurSite, reloaded.Status);
        Assert.Equal("B-099", reloaded.BadgeIssued);
        Assert.NotNull(reloaded.CheckInAt);
    }

    [Fact]
    public async Task CheckOut_OnlyAfterCheckIn()
    {
        var service = BuildService(out _, new DateTime(2026, 6, 23));
        var v = await service.CreateAsync(new Visitor { FullName = "X", Purpose = "p", ScheduledArrival = DateTime.UtcNow });

        // Tentative de sortie avant entrée
        Assert.False(await service.CheckOutAsync(v.Id, "PFSO"));

        await service.CheckInAsync(v.Id, "PFSO", "B-001");
        Assert.True(await service.CheckOutAsync(v.Id, "PFSO"));
    }
}
