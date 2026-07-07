using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class BriefingService : IBriefingService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public BriefingService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<IReadOnlyList<ShiftBriefing>> GetRecentAsync(int take = 30)
        => await _db.ShiftBriefings.AsNoTracking()
            .OrderByDescending(b => b.ShiftDate)
            .ThenByDescending(b => b.Slot)
            .Take(take)
            .ToListAsync();

    public Task<ShiftBriefing?> GetByIdAsync(int id)
        => _db.ShiftBriefings.FirstOrDefaultAsync(b => b.Id == id);

    public Task<ShiftBriefing?> GetLatestAsync()
        => _db.ShiftBriefings.AsNoTracking()
            .OrderByDescending(b => b.ShiftDate)
            .ThenByDescending(b => b.Slot)
            .FirstOrDefaultAsync();

    public async Task<ShiftBriefing> CreateAsync(ShiftBriefing input)
    {
        input.CreatedAt = DateTime.UtcNow;
        input.OutgoingAgent = input.OutgoingAgent?.Trim() ?? string.Empty;
        input.IncomingAgent = input.IncomingAgent?.Trim() ?? string.Empty;
        input.EventsSummary = input.EventsSummary?.Trim() ?? string.Empty;
        input.AttentionPoints = input.AttentionPoints?.Trim() ?? string.Empty;
        input.StandingOrders = input.StandingOrders?.Trim() ?? string.Empty;
        _db.ShiftBriefings.Add(input);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CreateShiftBriefing",
            $"{input.ShiftDate:yyyy-MM-dd} / {input.Slot} — {input.OutgoingAgent} → {input.IncomingAgent}");
        return input;
    }

    public async Task<bool> UpdateAsync(ShiftBriefing input, byte[] rowVersion)
    {
        var e = await _db.ShiftBriefings.FirstOrDefaultAsync(b => b.Id == input.Id);
        if (e is null) return false;
        _db.Entry(e).Property(nameof(ShiftBriefing.RowVersion)).OriginalValue = rowVersion;
        e.ShiftDate = input.ShiftDate;
        e.Slot = input.Slot;
        e.CurrentMarsecLevel = input.CurrentMarsecLevel;
        e.OutgoingAgent = input.OutgoingAgent?.Trim() ?? string.Empty;
        e.IncomingAgent = input.IncomingAgent?.Trim() ?? string.Empty;
        e.EventsSummary = input.EventsSummary?.Trim() ?? string.Empty;
        e.AttentionPoints = input.AttentionPoints?.Trim() ?? string.Empty;
        e.StandingOrders = input.StandingOrders?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateBriefing", $"#{e.Id}");
        return true;
    }

    public async Task<bool> AcknowledgeAsync(int id, string by)
    {
        var entity = await _db.ShiftBriefings.FirstOrDefaultAsync(b => b.Id == id);
        if (entity is null) return false;
        entity.AcknowledgedByIncoming = true;
        entity.AcknowledgedAt = DateTime.UtcNow;
        if (string.IsNullOrEmpty(entity.IncomingAgent))
            entity.IncomingAgent = by;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AcknowledgeBriefing", $"#{id} par {by}");
        return true;
    }

    public ShiftSlot DetermineCurrentSlot(DateTime now)
    {
        var h = now.Hour;
        if (h is >= 6 and < 14) return ShiftSlot.Matin;
        if (h is >= 14 and < 22) return ShiftSlot.ApresMidi;
        return ShiftSlot.Nuit;
    }
}
