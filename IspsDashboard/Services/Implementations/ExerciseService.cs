using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public class ExerciseService : IExerciseService
{
    private readonly ApplicationDbContext _db;

    public ExerciseService(ApplicationDbContext db) => _db = db;

    public int ComputeDaysRemaining(Exercise exercise)
    {
        var today = DateTime.UtcNow.Date;
        return (int)(exercise.PlannedDate.Date - today).TotalDays;
    }

    public ExerciseColor ComputeColor(Exercise exercise)
    {
        if (exercise.Status == ExerciseStatus.Realise)
            return ExerciseColor.Bleu;

        var days = ComputeDaysRemaining(exercise);
        if (days < 7) return ExerciseColor.Rouge;
        if (days <= 30) return ExerciseColor.Ambre;
        return ExerciseColor.Vert;
    }

    public string ComputeCountdownLabel(Exercise exercise)
    {
        if (exercise.Status == ExerciseStatus.Realise)
            return "✓ Réalisé";

        var days = ComputeDaysRemaining(exercise);
        if (days < 0) return $"En retard de {Math.Abs(days)} j";
        if (days == 0) return "Aujourd'hui";
        return $"J-{days}";
    }

    public ExerciseWithCountdown Wrap(Exercise exercise) => new()
    {
        Exercise = exercise,
        DaysRemaining = ComputeDaysRemaining(exercise),
        Color = ComputeColor(exercise),
        CountdownLabel = ComputeCountdownLabel(exercise)
    };

    public async Task<IReadOnlyList<Exercise>> GetUpcomingAsync(int daysAhead)
    {
        var today = DateTime.UtcNow.Date;
        var horizon = today.AddDays(daysAhead);
        return await _db.Exercises
            .Where(e => e.Status == ExerciseStatus.Planifie
                        && e.PlannedDate.Date >= today
                        && e.PlannedDate.Date <= horizon)
            .ToListAsync();
    }
}
