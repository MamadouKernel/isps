using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Implementations;
using IspsDashboard.Services.Interfaces;
using IspsDashboard.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace IspsDashboard.Tests;

public class MarsecServiceTests
{
    private MarsecService BuildService(out IspsDashboard.Data.ApplicationDbContext db, int initialLevel = 1)
    {
        db = TestHelpers.CreateInMemoryDb();
        db.DashboardSettings.Add(new DashboardSettings { Id = 1, IspsLevel = initialLevel });
        db.SaveChanges();
        var audit = new AuditService(db, new HttpContextAccessor());
        var accessPasses = new AccessPassService(db, new TestHelpers.FixedClock(DateTime.UtcNow.Date), new TestHelpers.NoOpAuditService());
        var dashboard = new DashboardService(db, new ExerciseService(db), accessPasses, new MemoryCache(new MemoryCacheOptions()));
        return new MarsecService(db, audit, dashboard);
    }

    [Fact]
    public async Task RequestChangeAsync_UpdatesLevelAndCreatesChecklist()
    {
        var service = BuildService(out var db, initialLevel: 1);

        var change = await service.RequestChangeAsync(2, "Renseignement PAA",
            "Directive 2026-018", "PFSO", "user-1");

        Assert.Equal(1, change.FromLevel);
        Assert.Equal(2, change.ToLevel);
        Assert.NotEmpty(change.ChecklistItems);
        Assert.Equal(2, db.DashboardSettings.First().IspsLevel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(-1)]
    public async Task RequestChangeAsync_RejectsInvalidLevel(int badLevel)
    {
        var service = BuildService(out _);
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.RequestChangeAsync(badLevel, "x", "y", "z", null));
    }

    [Fact]
    public void GetChecklistTemplate_ReturnsMoreActionsForHigherLevels()
    {
        var service = BuildService(out _);
        var l1 = service.GetChecklistTemplate(1).Count;
        var l2 = service.GetChecklistTemplate(2).Count;
        var l3 = service.GetChecklistTemplate(3).Count;
        Assert.True(l2 > l1);
        Assert.True(l3 >= l2);
    }
}
