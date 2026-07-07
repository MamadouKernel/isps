using IspsDashboard.Data;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Tests;

internal static class TestHelpers
{
    public static ApplicationDbContext CreateInMemoryDb(string? name = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;
        return new ApplicationDbContext(options);
    }

    public sealed class FixedClock : IClock
    {
        public FixedClock(DateTime fixedNow) => Now = fixedNow;
        public DateTime Now { get; }
        public DateTime Today => Now.Date;
        public DateTime ToLocal(DateTime utc) => utc;
    }

    public sealed class NoOpAuditService : IAuditService
    {
        public Task LogAsync(string action, string detail, string? userId = null, string? userName = null)
            => Task.CompletedTask;
    }
}
