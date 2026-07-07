using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IspsDashboard.Tests;

public class VehicleAccessServiceTests
{
    private VehicleAccessService Build(out IspsDashboard.Data.ApplicationDbContext db, DateTime today)
    {
        db = TestHelpers.CreateInMemoryDb();
        var clock = new TestHelpers.FixedClock(today);
        var audit = new AuditService(db, new HttpContextAccessor());
        return new VehicleAccessService(db, clock, audit);
    }

    [Fact]
    public async Task CreateAsync_GeneratesSequentialReferences()
    {
        var service = Build(out _, new DateTime(2026, 6, 23));
        var a = await service.CreateAsync(new VehicleAccess { Plate = "AB-123-CD", OccurredAt = new DateTime(2026, 6, 23) });
        var b = await service.CreateAsync(new VehicleAccess { Plate = "EF-456-GH", OccurredAt = new DateTime(2026, 6, 23) });
        Assert.Equal("ACC-2026-0001", a.Reference);
        Assert.Equal("ACC-2026-0002", b.Reference);
    }

    [Fact]
    public async Task GetTodayAsync_OnlyReturnsTodayMovements()
    {
        var today = new DateTime(2026, 6, 23);
        var service = Build(out var db, today);
        db.VehicleAccesses.AddRange(
            new VehicleAccess { Reference = "ACC-2026-0001", Plate = "A", OccurredAt = today.AddHours(9) },
            new VehicleAccess { Reference = "ACC-2026-0002", Plate = "B", OccurredAt = today.AddDays(-1) });
        await db.SaveChangesAsync();

        var result = await service.GetTodayAsync();
        Assert.Single(result);
        Assert.Equal("A", result[0].Plate);
    }

    [Fact]
    public async Task SearchAsync_FiltersByPlateQuery()
    {
        var today = new DateTime(2026, 6, 23);
        var service = Build(out var db, today);
        db.VehicleAccesses.AddRange(
            new VehicleAccess { Reference = "ACC-2026-0001", Plate = "AB-123-CD", OccurredAt = today },
            new VehicleAccess { Reference = "ACC-2026-0002", Plate = "ZZ-999-ZZ", OccurredAt = today });
        await db.SaveChangesAsync();

        var result = await service.SearchAsync(null, null, null, "123");
        Assert.Single(result);
        Assert.Equal("AB-123-CD", result[0].Plate);
    }
}
