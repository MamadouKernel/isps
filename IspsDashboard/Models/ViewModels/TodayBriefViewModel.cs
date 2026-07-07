using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;

namespace IspsDashboard.Models.ViewModels;

public class TodayBriefViewModel
{
    public int OpenIncidents { get; init; }
    public int VisitorsOnSite { get; init; }
    public int OverduePatrols { get; init; }
    public IReadOnlyList<Habilitation> ExpiringHabilitations { get; init; } = Array.Empty<Habilitation>();
    public IReadOnlyList<Habilitation> ExpiredHabilitations { get; init; } = Array.Empty<Habilitation>();
    public IReadOnlyList<Exercise> UrgentExercises { get; init; } = Array.Empty<Exercise>();
    public IReadOnlyList<NonConformity> OpenNonConformities { get; init; } = Array.Empty<NonConformity>();
    public IReadOnlyList<AccessPass> ExpiringAccessPasses { get; init; } = Array.Empty<AccessPass>();
    public int CurrentMarsecLevel { get; init; }
    public IHabilitationService HabilitationService { get; init; } = null!;
}
