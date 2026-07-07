using IspsDashboard.Models.Entities;

namespace IspsDashboard.Models.ViewModels;

public class DashboardViewModel
{
    public DashboardSettings Settings { get; set; } = new();
    public List<KpiCard> KpiCards { get; set; } = new();
    public List<Gauge> Gauges { get; set; } = new();
    public List<ProgressBar> ProgressBars { get; set; } = new();
    public List<KpiTableRow> KpiTable { get; set; } = new();
    public List<Agent> Agents { get; set; } = new();
    public List<ExerciseWithCountdown> Trainings { get; set; } = new();
    public ExerciseWithCountdown? GrandNature { get; set; }
    public List<ExerciseWithCountdown> CrisisExercises { get; set; } = new();
    public List<AccessPassWithCountdown> ExpiringAccessPasses { get; set; } = new();
    public int ActiveAccessPasses { get; set; }

    public int PresentAgents => Agents.Count(a => a.IsPresent);
    public int TotalAgents => Agents.Count;

    public Dictionary<string, double> RadarScores { get; set; } = new();
}
