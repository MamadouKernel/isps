using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class ContactService : IContactService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public ContactService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<IReadOnlyList<ExternalContact>> GetAllAsync(ContactType? type = null)
    {
        IQueryable<ExternalContact> q = _db.ExternalContacts.AsNoTracking();
        if (type is { } t) q = q.Where(c => c.Type == t);
        return await q.OrderBy(c => c.Type).ThenBy(c => c.Name).ToListAsync();
    }

    public Task<ExternalContact?> GetByIdAsync(int id)
        => _db.ExternalContacts
            .Include(c => c.Interactions.OrderByDescending(i => i.OccurredAt).Take(50))
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<ExternalContact> CreateAsync(ExternalContact input)
    {
        input.CreatedAt = DateTime.UtcNow;
        input.Role = input.Role?.Trim() ?? string.Empty;
        input.PrimaryPhone = input.PrimaryPhone?.Trim() ?? string.Empty;
        input.EmergencyPhone = input.EmergencyPhone?.Trim() ?? string.Empty;
        input.Email = input.Email?.Trim() ?? string.Empty;
        input.Address = input.Address?.Trim() ?? string.Empty;
        input.RadioChannel = input.RadioChannel?.Trim() ?? string.Empty;
        input.Notes = input.Notes?.Trim() ?? string.Empty;
        _db.ExternalContacts.Add(input);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CreateExternalContact", $"{input.Type} — {input.Name}");
        return input;
    }

    public async Task<bool> UpdateAsync(ExternalContact input)
    {
        var existing = await _db.ExternalContacts.FirstOrDefaultAsync(c => c.Id == input.Id);
        if (existing is null) return false;
        existing.Name = input.Name.Trim();
        existing.Type = input.Type;
        existing.Role = input.Role?.Trim() ?? string.Empty;
        existing.PrimaryPhone = input.PrimaryPhone?.Trim() ?? string.Empty;
        existing.EmergencyPhone = input.EmergencyPhone?.Trim() ?? string.Empty;
        existing.Email = input.Email?.Trim() ?? string.Empty;
        existing.Address = input.Address?.Trim() ?? string.Empty;
        existing.RadioChannel = input.RadioChannel?.Trim() ?? string.Empty;
        existing.IsEmergency24x7 = input.IsEmergency24x7;
        existing.Notes = input.Notes?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateExternalContact", existing.Name);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.ExternalContacts.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null) return false;
        _db.ExternalContacts.Remove(entity);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("DeleteExternalContact", entity.Name);
        return true;
    }

    public async Task<ContactInteraction> LogInteractionAsync(int contactId, ContactInteraction entry)
    {
        entry.ExternalContactId = contactId;
        entry.OccurredAt = entry.OccurredAt == default ? DateTime.UtcNow : entry.OccurredAt;
        entry.HandledBy = entry.HandledBy?.Trim() ?? string.Empty;
        entry.Summary = entry.Summary?.Trim() ?? string.Empty;
        _db.ContactInteractions.Add(entry);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("LogContactInteraction", $"#{contactId} — {entry.Subject}");
        return entry;
    }

    public async Task<IReadOnlyList<ContactInteraction>> GetRecentInteractionsAsync(int take = 20)
        => await _db.ContactInteractions.AsNoTracking()
            .Include(i => i.Contact)
            .OrderByDescending(i => i.OccurredAt)
            .Take(take)
            .ToListAsync();
}
