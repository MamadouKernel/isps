namespace IspsDashboard.Models.ViewModels;

public enum AlertLevel { Info, Warning, Critical }

public class AlertItem
{
    public AlertLevel Level { get; init; }
    public string Icon { get; init; } = "•";
    public string Title { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;
    public string Url { get; init; } = "#";
}

public class AlertCenterViewModel
{
    public IReadOnlyList<AlertItem> Alerts { get; init; } = Array.Empty<AlertItem>();
    public int CriticalCount => Alerts.Count(a => a.Level == AlertLevel.Critical);
    public int TotalCount => Alerts.Count;
}
