using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class HabilitationService : IHabilitationService
{
    private readonly ApplicationDbContext _db;
    private readonly IClock _clock;
    private readonly IAuditService _audit;

    public HabilitationService(ApplicationDbContext db, IClock clock, IAuditService audit)
    {
        _db = db;
        _clock = clock;
        _audit = audit;
    }

    public async Task<Habilitation> CreateAsync(int agentId, Habilitation input)
    {
        var entity = new Habilitation
        {
            AgentId = agentId,
            Category = input.Category,
            Title = input.Title.Trim(),
            Issuer = input.Issuer?.Trim() ?? string.Empty,
            ObtainedAt = input.ObtainedAt,
            ExpiresAt = input.ExpiresAt,
            Reference = input.Reference?.Trim() ?? string.Empty,
            DocumentPath = input.DocumentPath ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };
        _db.Habilitations.Add(entity);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CreateHabilitation", $"Agent #{agentId} — {entity.Title}");
        return entity;
    }

    public Task<Habilitation?> GetByIdAsync(int id)
        => _db.Habilitations.Include(h => h.Agent).FirstOrDefaultAsync(h => h.Id == id);

    public async Task<int> UpdateAsync(Habilitation input)
    {
        var e = await _db.Habilitations.FirstOrDefaultAsync(h => h.Id == input.Id);
        if (e is null) return 0;
        e.Category = input.Category;
        e.Title = input.Title.Trim();
        e.Issuer = input.Issuer?.Trim() ?? string.Empty;
        e.ObtainedAt = input.ObtainedAt;
        e.ExpiresAt = input.ExpiresAt;
        e.Reference = input.Reference?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateHabilitation", $"Agent #{e.AgentId} — {e.Title}");
        return e.AgentId;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Habilitations.FirstOrDefaultAsync(h => h.Id == id);
        if (entity is null) return false;
        _db.Habilitations.Remove(entity);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("DeleteHabilitation", $"#{id}");
        return true;
    }

    public async Task<IReadOnlyList<Habilitation>> GetAllAsync()
        => await _db.Habilitations
            .AsNoTracking()
            .Include(h => h.Agent)
            .OrderBy(h => h.Agent!.Name).ThenBy(h => h.Title)
            .ToListAsync();

    public async Task<IReadOnlyList<Habilitation>> GetExpiringSoonAsync(int daysAhead = 60)
    {
        var horizon = _clock.Today.AddDays(daysAhead);
        return await _db.Habilitations
            .AsNoTracking()
            .Include(h => h.Agent)
            .Where(h => h.ExpiresAt >= _clock.Today && h.ExpiresAt <= horizon)
            .OrderBy(h => h.ExpiresAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Habilitation>> GetExpiredAsync()
        => await _db.Habilitations
            .AsNoTracking()
            .Include(h => h.Agent)
            .Where(h => h.ExpiresAt < _clock.Today)
            .OrderByDescending(h => h.ExpiresAt)
            .ToListAsync();

    public int DaysToExpiry(Habilitation habilitation)
        => (int)(habilitation.ExpiresAt.Date - _clock.Today).TotalDays;

    private const int UrgentThresholdDays = 30;
    private const int SoonThresholdDays = 60;

    public HabilitationStatus ClassifyExpiry(Habilitation habilitation)
    {
        var days = DaysToExpiry(habilitation);
        if (days < 0) return HabilitationStatus.Expiree;
        if (days < UrgentThresholdDays) return HabilitationStatus.Urgent;
        if (days <= SoonThresholdDays) return HabilitationStatus.Bientot;
        return HabilitationStatus.Valide;
    }
}
