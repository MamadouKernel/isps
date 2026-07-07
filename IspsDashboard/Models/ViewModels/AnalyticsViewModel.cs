namespace IspsDashboard.Models.ViewModels;

public class AnalyticsViewModel
{
    public List<string> MonthLabels { get; set; } = new();
    public List<int> IncidentsPerMonth { get; set; } = new();
    public List<int> VehiclesPerMonth { get; set; } = new();
    public List<int> ExercisesRealisedPerMonth { get; set; } = new();
    public List<int> NonConformitiesPerMonth { get; set; } = new();
    public List<int> VisitorsPerMonth { get; set; } = new();
    public double CctvAvailability { get; set; }
    public int TotalIncidents12m { get; set; }
    public int TotalVehicles12m { get; set; }
    public int OpenNonConformities { get; set; }
    public int HabilitationsExpiringSoon { get; set; }
}
