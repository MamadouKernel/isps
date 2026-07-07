using System.Globalization;
using IspsDashboard.Data;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class AnalyticsService : IAnalyticsService
{
    private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr-FR");

    private readonly ApplicationDbContext _db;
    private readonly IClock _clock;
    private readonly ICameraService _cameras;
    private readonly IHabilitationService _habilitations;

    public AnalyticsService(
        ApplicationDbContext db,
        IClock clock,
        ICameraService cameras,
        IHabilitationService habilitations)
    {
        _db = db;
        _clock = clock;
        _cameras = cameras;
        _habilitations = habilitations;
    }

    public async Task<AnalyticsViewModel> Build12MonthAsync()
    {
        var today = _clock.Today;
        var start = new DateTime(today.Year, today.Month, 1).AddMonths(-11);

        // 12 fenêtres mensuelles [début, fin[
        var windows = Enumerable.Range(0, 12)
            .Select(i => start.AddMonths(i))
            .Select(m => (Start: m, End: m.AddMonths(1), Label: m.ToString("MMM yy", Fr)))
            .ToList();

        var incidents = await _db.Incidents.AsNoTracking()
            .Where(i => i.OccurredAt >= start).Select(i => i.OccurredAt).ToListAsync();
        var vehicles = await _db.VehicleAccesses.AsNoTracking()
            .Where(v => v.OccurredAt >= start).Select(v => v.OccurredAt).ToListAsync();
        var exercises = await _db.Exercises.AsNoTracking()
            .Where(e => e.Status == ExerciseStatus.Realise && e.PlannedDate >= start)
            .Select(e => e.PlannedDate).ToListAsync();
        var ncs = await _db.NonConformities.AsNoTracking()
            .Where(n => n.IdentifiedAt >= start).Select(n => n.IdentifiedAt).ToListAsync();
        var visitors = await _db.Visitors.AsNoTracking()
            .Where(v => v.ScheduledArrival >= start).Select(v => v.ScheduledArrival).ToListAsync();

        int CountIn(IEnumerable<DateTime> source, DateTime s, DateTime e)
            => source.Count(d => d >= s && d < e);

        var vm = new AnalyticsViewModel
        {
            MonthLabels = windows.Select(w => w.Label).ToList(),
            IncidentsPerMonth = windows.Select(w => CountIn(incidents, w.Start, w.End)).ToList(),
            VehiclesPerMonth = windows.Select(w => CountIn(vehicles, w.Start, w.End)).ToList(),
            ExercisesRealisedPerMonth = windows.Select(w => CountIn(exercises, w.Start, w.End)).ToList(),
            NonConformitiesPerMonth = windows.Select(w => CountIn(ncs, w.Start, w.End)).ToList(),
            VisitorsPerMonth = windows.Select(w => CountIn(visitors, w.Start, w.End)).ToList(),
            CctvAvailability = await _cameras.AvailabilityPercentAsync(),
            TotalIncidents12m = incidents.Count,
            TotalVehicles12m = vehicles.Count,
            OpenNonConformities = await _db.NonConformities.CountAsync(n =>
                n.Status != NonConformityStatus.Levee && n.Status != NonConformityStatus.Rejetee),
            HabilitationsExpiringSoon = (await _habilitations.GetExpiringSoonAsync(60)).Count
        };
        return vm;
    }
}
