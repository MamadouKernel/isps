using IspsDashboard.Data;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class TodayBriefService : ITodayBriefService
{
    private readonly ApplicationDbContext _db;
    private readonly IIncidentService _incidents;
    private readonly IVisitorService _visitors;
    private readonly IPatrolService _patrols;
    private readonly IHabilitationService _habilitations;
    private readonly INonConformityService _nonConformities;
    private readonly IMarsecService _marsec;
    private readonly IAccessPassService _accessPasses;
    private readonly IClock _clock;

    public TodayBriefService(
        ApplicationDbContext db,
        IIncidentService incidents,
        IVisitorService visitors,
        IPatrolService patrols,
        IHabilitationService habilitations,
        INonConformityService nonConformities,
        IMarsecService marsec,
        IAccessPassService accessPasses,
        IClock clock)
    {
        _db = db;
        _incidents = incidents;
        _visitors = visitors;
        _patrols = patrols;
        _habilitations = habilitations;
        _nonConformities = nonConformities;
        _marsec = marsec;
        _accessPasses = accessPasses;
        _clock = clock;
    }

    public async Task<TodayBriefViewModel> BuildAsync()
    {
        var openIncidents = await _incidents.CountByStatusAsync(IncidentStatus.Ouvert)
                          + await _incidents.CountByStatusAsync(IncidentStatus.EnInvestigation);
        var onSite = (await _visitors.GetOnSiteAsync()).Count;
        var overdue = (await _patrols.GetOverdueAsync()).Count;
        var expiringSoon = await _habilitations.GetExpiringSoonAsync(60);
        var expired = await _habilitations.GetExpiredAsync();
        var openNc = await _nonConformities.GetOpenAsync();
        var marsecLevel = await _marsec.GetCurrentLevelAsync();
        var expiringPasses = await _accessPasses.GetExpiringSoonAsync(30);

        var horizon7Days = _clock.Today.AddDays(7);
        var urgentExercises = await _db.Exercises.AsNoTracking()
            .Where(e => e.Status == ExerciseStatus.Planifie
                        && e.PlannedDate.Date <= horizon7Days)
            .OrderBy(e => e.PlannedDate)
            .Take(5)
            .ToListAsync();

        return new TodayBriefViewModel
        {
            OpenIncidents = openIncidents,
            VisitorsOnSite = onSite,
            OverduePatrols = overdue,
            ExpiringHabilitations = expiringSoon,
            ExpiredHabilitations = expired,
            UrgentExercises = urgentExercises,
            OpenNonConformities = openNc,
            ExpiringAccessPasses = expiringPasses,
            CurrentMarsecLevel = marsecLevel,
            HabilitationService = _habilitations
        };
    }
}
