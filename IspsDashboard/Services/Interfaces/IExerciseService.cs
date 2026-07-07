using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;

namespace IspsDashboard.Services.Interfaces;

public interface IExerciseService
{
    ExerciseColor ComputeColor(Exercise exercise);
    int ComputeDaysRemaining(Exercise exercise);
    string ComputeCountdownLabel(Exercise exercise);
    ExerciseWithCountdown Wrap(Exercise exercise);
    Task<IReadOnlyList<Exercise>> GetUpcomingAsync(int daysAhead);
}
