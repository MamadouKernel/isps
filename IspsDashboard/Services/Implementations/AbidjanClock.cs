using IspsDashboard.Services.Interfaces;

namespace IspsDashboard.Services.Implementations;

/// <summary>
/// Horloge fixe sur la time zone Abidjan (Greenwich Mean Time, UTC+0, sans heure d'été).
/// Côte d'Ivoire ne pratique pas le passage à l'heure d'été.
/// </summary>
public sealed class AbidjanClock : IClock
{
    // "GMT Standard Time" sur Windows, "Africa/Abidjan" sur Linux.
    private static readonly TimeZoneInfo AbidjanZone = ResolveZone();

    public DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, AbidjanZone);
    public DateTime Today => Now.Date;

    public DateTime ToLocal(DateTime utc)
    {
        var asUtc = utc.Kind == DateTimeKind.Utc ? utc : DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(asUtc, AbidjanZone);
    }

    private static TimeZoneInfo ResolveZone()
    {
        foreach (var id in new[] { "Africa/Abidjan", "GMT Standard Time", "Greenwich Standard Time" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
        }
        return TimeZoneInfo.Utc;
    }
}
