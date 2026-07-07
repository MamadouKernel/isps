using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class RexService : IRexService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public RexService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public Task<ExerciseRex?> GetForExerciseAsync(int exerciseId)
        => _db.ExerciseRexes.Include(r => r.Exercise).FirstOrDefaultAsync(r => r.ExerciseId == exerciseId);

    public async Task<ExerciseRex> CreateOrUpdateAsync(ExerciseRex input)
    {
        var existing = await _db.ExerciseRexes.FirstOrDefaultAsync(r => r.ExerciseId == input.ExerciseId);
        if (existing is null)
        {
            input.WrittenAt = DateTime.UtcNow;
            input.Sequence = input.Sequence?.Trim() ?? string.Empty;
            input.PositivePoints = input.PositivePoints?.Trim() ?? string.Empty;
            input.ImprovementPoints = input.ImprovementPoints?.Trim() ?? string.Empty;
            input.CorrectiveActions = input.CorrectiveActions?.Trim() ?? string.Empty;
            input.FollowUp = input.FollowUp?.Trim() ?? string.Empty;
            input.WrittenBy = input.WrittenBy?.Trim() ?? string.Empty;
            _db.ExerciseRexes.Add(input);
        }
        else
        {
            existing.Sequence = input.Sequence?.Trim() ?? string.Empty;
            existing.PositivePoints = input.PositivePoints?.Trim() ?? string.Empty;
            existing.ImprovementPoints = input.ImprovementPoints?.Trim() ?? string.Empty;
            existing.CorrectiveActions = input.CorrectiveActions?.Trim() ?? string.Empty;
            existing.FollowUp = input.FollowUp?.Trim() ?? string.Empty;
            existing.WrittenBy = input.WrittenBy?.Trim() ?? string.Empty;
            existing.WrittenAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync(existing is null ? "CreateRex" : "UpdateRex", $"Exercise #{input.ExerciseId}");
        return existing ?? input;
    }

    public async Task<IReadOnlyList<ExerciseRex>> GetRecentAsync(int take = 10)
        => await _db.ExerciseRexes.AsNoTracking()
            .Include(r => r.Exercise)
            .OrderByDescending(r => r.WrittenAt)
            .Take(take)
            .ToListAsync();
}
