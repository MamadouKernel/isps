using IspsDashboard.Models.Entities;

namespace IspsDashboard.Services.Interfaces;

public interface IRexService
{
    Task<ExerciseRex?> GetForExerciseAsync(int exerciseId);
    Task<ExerciseRex> CreateOrUpdateAsync(ExerciseRex input);
    Task<IReadOnlyList<ExerciseRex>> GetRecentAsync(int take = 10);
}
