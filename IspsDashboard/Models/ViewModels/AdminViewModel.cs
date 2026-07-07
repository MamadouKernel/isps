using IspsDashboard.Models.Entities;

namespace IspsDashboard.Models.ViewModels;

public class AdminViewModel
{
    public DashboardSettings Settings { get; set; } = new();
    public List<KpiCard> KpiCards { get; set; } = new();
    public List<Gauge> Gauges { get; set; } = new();
    public List<ProgressBar> ProgressBars { get; set; } = new();
    public List<Agent> Agents { get; set; } = new();
    public List<Exercise> Trainings { get; set; } = new();
    public Exercise? GrandNature { get; set; }
    public List<Exercise> CrisisExercises { get; set; } = new();
    public List<KpiTableRow> KpiTable { get; set; } = new();
}
