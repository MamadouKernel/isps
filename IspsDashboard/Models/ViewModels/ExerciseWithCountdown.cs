using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.ViewModels;

public class ExerciseWithCountdown
{
    public Exercise Exercise { get; set; } = null!;
    public int DaysRemaining { get; set; }
    public ExerciseColor Color { get; set; }
    public string CountdownLabel { get; set; } = string.Empty;
}
