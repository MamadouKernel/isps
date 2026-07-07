using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Implementations;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IspsDashboard.Tests;

public class HabilitationServiceTests
{
    private HabilitationService BuildService(DateTime fixedToday)
    {
        var db = TestHelpers.CreateInMemoryDb();
        var clock = new TestHelpers.FixedClock(fixedToday);
        var audit = new AuditService(db, new HttpContextAccessor());
        return new HabilitationService(db, clock, audit);
    }

    [Theory]
    [InlineData(90, HabilitationStatus.Valide)]
    [InlineData(60, HabilitationStatus.Bientot)]
    [InlineData(45, HabilitationStatus.Bientot)]
    [InlineData(29, HabilitationStatus.Urgent)]
    [InlineData(1, HabilitationStatus.Urgent)]
    [InlineData(0, HabilitationStatus.Urgent)]
    [InlineData(-1, HabilitationStatus.Expiree)]
    public void ClassifyExpiry_Returns_ExpectedBucket(int daysAhead, HabilitationStatus expected)
    {
        var today = new DateTime(2026, 6, 23);
        var service = BuildService(today);
        var h = new Habilitation { ExpiresAt = today.AddDays(daysAhead) };
        Assert.Equal(expected, service.ClassifyExpiry(h));
    }

    [Fact]
    public void DaysToExpiry_NegativeForPast_PositiveForFuture()
    {
        var today = new DateTime(2026, 6, 23);
        var service = BuildService(today);
        Assert.Equal(10, service.DaysToExpiry(new Habilitation { ExpiresAt = today.AddDays(10) }));
        Assert.Equal(-5, service.DaysToExpiry(new Habilitation { ExpiresAt = today.AddDays(-5) }));
    }

    [Fact]
    public async Task GetExpiringSoonAsync_ExcludesAlreadyExpired_AndBeyondHorizon()
    {
        var today = new DateTime(2026, 6, 23);
        var db = TestHelpers.CreateInMemoryDb();
        db.Agents.Add(new Agent { Id = 1, Name = "A1", Position = 1 });
        db.Habilitations.AddRange(
            new Habilitation { Id = 1, AgentId = 1, Title = "H1", ExpiresAt = today.AddDays(-5), ObtainedAt = today.AddYears(-1) }, // expirée
            new Habilitation { Id = 2, AgentId = 1, Title = "H2", ExpiresAt = today.AddDays(30), ObtainedAt = today.AddYears(-1) },
            new Habilitation { Id = 3, AgentId = 1, Title = "H3", ExpiresAt = today.AddDays(120), ObtainedAt = today.AddYears(-1) }); // au-delà
        await db.SaveChangesAsync();

        var clock = new TestHelpers.FixedClock(today);
        var audit = new AuditService(db, new HttpContextAccessor());
        var service = new HabilitationService(db, clock, audit);

        var result = await service.GetExpiringSoonAsync(60);

        Assert.Single(result);
        Assert.Equal(2, result[0].Id);
    }
}
