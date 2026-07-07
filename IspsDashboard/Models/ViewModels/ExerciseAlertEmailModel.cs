using IspsDashboard.Models.Entities;

namespace IspsDashboard.Models.ViewModels;

public class ExerciseAlertEmailModel
{
    public Exercise Exercise { get; init; } = null!;
    public int DaysRemaining { get; init; }
    public string TerminalTitle { get; init; } = "Côte d'Ivoire Terminal";
}
