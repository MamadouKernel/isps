using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.ViewModels;

public class AccessPassWithCountdown
{
    public AccessPass Pass { get; set; } = null!;
    public int DaysRemaining { get; set; }
    public KpiCardColor Color { get; set; }
    public string CountdownLabel { get; set; } = string.Empty;
}
