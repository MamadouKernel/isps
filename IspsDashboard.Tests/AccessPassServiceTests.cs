using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Implementations;
using Xunit;

namespace IspsDashboard.Tests;

public class AccessPassServiceTests
{
    private static AccessPassService CreateService(DateTime today, IspsDashboard.Data.ApplicationDbContext? db = null)
        => new(db ?? TestHelpers.CreateInMemoryDb(), new TestHelpers.FixedClock(today), new TestHelpers.NoOpAuditService());

    [Theory]
    [InlineData(45, KpiCardColor.Vert)]     // > 30 jours
    [InlineData(31, KpiCardColor.Vert)]
    [InlineData(30, KpiCardColor.Ambre)]    // dans 30 jours
    [InlineData(8, KpiCardColor.Ambre)]
    [InlineData(7, KpiCardColor.Rouge)]     // <= 7 jours
    [InlineData(0, KpiCardColor.Rouge)]
    [InlineData(-3, KpiCardColor.Rouge)]    // déjà expiré
    public void Wrap_ShouldReturnExpectedColor_ForDaysRemaining(int daysFromToday, KpiCardColor expected)
    {
        var today = new DateTime(2026, 7, 7);
        var service = CreateService(today);
        var pass = new AccessPass { EndDate = today.AddDays(daysFromToday), LastName = "Diallo" };

        var wrapped = service.Wrap(pass);

        Assert.Equal(expected, wrapped.Color);
        Assert.Equal(daysFromToday, wrapped.DaysRemaining);
    }

    [Theory]
    [InlineData(10, "J-10")]
    [InlineData(1, "J-1")]
    [InlineData(0, "Aujourd'hui")]
    [InlineData(-5, "Expiré depuis 5 j")]
    public void Wrap_ShouldFormatCountdownLabel(int daysFromToday, string expected)
    {
        var today = new DateTime(2026, 7, 7);
        var service = CreateService(today);
        var pass = new AccessPass { EndDate = today.AddDays(daysFromToday), LastName = "Diallo" };

        Assert.Equal(expected, service.Wrap(pass).CountdownLabel);
    }

    [Fact]
    public async Task GetExpiringSoonAsync_ShouldExcludeExpiredAndRevokedAndFarFuturePasses()
    {
        var today = new DateTime(2026, 7, 7);
        var db = TestHelpers.CreateInMemoryDb();
        db.AccessPasses.AddRange(
            new AccessPass { LastName = "ExpireDansDix", EndDate = today.AddDays(10), IssueDate = today },
            new AccessPass { LastName = "DejaExpire", EndDate = today.AddDays(-2), IssueDate = today.AddDays(-100) },
            new AccessPass { LastName = "Revoque", EndDate = today.AddDays(5), IssueDate = today, Revoked = true },
            new AccessPass { LastName = "LoinDansLeFutur", EndDate = today.AddDays(200), IssueDate = today }
        );
        await db.SaveChangesAsync();

        var service = CreateService(today, db);
        var result = await service.GetExpiringSoonAsync(30);

        var name = Assert.Single(result).LastName;
        Assert.Equal("ExpireDansDix", name);
    }

    [Fact]
    public void DaysToExpiry_ShouldComputeWholeDaysBetweenTodayAndEndDate()
    {
        var today = new DateTime(2026, 7, 7);
        var service = CreateService(today);
        var pass = new AccessPass { EndDate = today.AddDays(17), LastName = "Diallo" };

        Assert.Equal(17, service.DaysToExpiry(pass));
    }
}
