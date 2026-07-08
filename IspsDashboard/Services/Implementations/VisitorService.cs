using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class VisitorService : IVisitorService
{
    private readonly ApplicationDbContext _db;
    private readonly IClock _clock;
    private readonly IAuditService _audit;

    public VisitorService(ApplicationDbContext db, IClock clock, IAuditService audit)
    {
        _db = db;
        _clock = clock;
        _audit = audit;
    }

    public async Task<IReadOnlyList<Visitor>> GetTodayAsync()
    {
        var today = _clock.Today;
        var tomorrow = today.AddDays(1);
        return await _db.Visitors.AsNoTracking()
            .Where(v => v.ScheduledArrival >= today && v.ScheduledArrival < tomorrow)
            .OrderBy(v => v.ScheduledArrival)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Visitor>> GetOnSiteAsync()
        => await _db.Visitors.AsNoTracking()
            .Where(v => v.Status == VisitorStatus.SurSite)
            .OrderBy(v => v.CheckInAt)
            .ToListAsync();

    public Task<int> CountOnSiteAsync()
        => _db.Visitors.CountAsync(v => v.Status == VisitorStatus.SurSite);

    public async Task<IReadOnlyList<Visitor>> SearchAsync(VisitorStatus? status, DateTime? from, DateTime? to)
    {
        IQueryable<Visitor> query = _db.Visitors.AsNoTracking();
        if (status is { } s) query = query.Where(v => v.Status == s);
        if (from is { } f) query = query.Where(v => v.ScheduledArrival >= f);
        if (to is { } t) query = query.Where(v => v.ScheduledArrival <= t.AddDays(1));
        return await query.OrderByDescending(v => v.ScheduledArrival).Take(500).ToListAsync();
    }

    public Task<Visitor?> GetByIdAsync(int id) => _db.Visitors.FirstOrDefaultAsync(v => v.Id == id);

    public async Task<Visitor> CreateAsync(Visitor input)
    {
        // Entité neuve à partir des seuls champs légitimes d'une pré-inscription : ne jamais
        // persister input tel quel (voir AuditCampaignService.CreateAsync pour le pourquoi) —
        // sinon un champ de formulaire ajouté peut forger un check-in/check-out déjà effectué,
        // ou masquer instantanément le visiteur via IsDeleted.
        var entity = new Visitor
        {
            FullName = input.FullName.Trim(),
            Company = input.Company?.Trim() ?? string.Empty,
            IdDocumentNumber = input.IdDocumentNumber?.Trim() ?? string.Empty,
            Phone = input.Phone?.Trim() ?? string.Empty,
            VehiclePlate = input.VehiclePlate?.Trim() ?? string.Empty,
            ScheduledArrival = input.ScheduledArrival,
            ScheduledDeparture = input.ScheduledDeparture,
            Purpose = input.Purpose.Trim(),
            Host = input.Host?.Trim() ?? string.Empty,
            EscortedBy = input.EscortedBy?.Trim() ?? string.Empty,
            Status = VisitorStatus.PreEnregistre,
            Notes = input.Notes?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };
        _db.Visitors.Add(entity);
        await ReferenceGenerator.SaveWithUniqueReferenceAsync(_db, r => entity.Reference = r, () => NextReference(entity.ScheduledArrival.Year));
        await _audit.LogAsync("CreateVisitor", $"{entity.Reference} — {entity.FullName}");
        return entity;
    }

    public async Task<bool> UpdateAsync(Visitor input, byte[] rowVersion)
    {
        var e = await _db.Visitors.FirstOrDefaultAsync(v => v.Id == input.Id);
        if (e is null) return false;
        _db.Entry(e).Property(nameof(Visitor.RowVersion)).OriginalValue = rowVersion;
        e.FullName = input.FullName.Trim();
        e.Company = input.Company?.Trim() ?? string.Empty;
        e.IdDocumentNumber = input.IdDocumentNumber?.Trim() ?? string.Empty;
        e.Phone = input.Phone?.Trim() ?? string.Empty;
        e.VehiclePlate = input.VehiclePlate?.Trim() ?? string.Empty;
        e.ScheduledArrival = input.ScheduledArrival;
        e.ScheduledDeparture = input.ScheduledDeparture;
        e.Purpose = input.Purpose?.Trim() ?? string.Empty;
        e.Host = input.Host?.Trim() ?? string.Empty;
        e.EscortedBy = input.EscortedBy?.Trim() ?? string.Empty;
        e.Notes = input.Notes?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateVisitor", e.Reference);
        return true;
    }

    public async Task<bool> CheckInAsync(int id, string by, string badgeIssued)
    {
        var entity = await _db.Visitors.FirstOrDefaultAsync(v => v.Id == id);
        if (entity is null || entity.Status == VisitorStatus.Sorti) return false;
        entity.Status = VisitorStatus.SurSite;
        entity.CheckInAt = DateTime.UtcNow;
        entity.CheckedInBy = by;
        entity.BadgeIssued = badgeIssued?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CheckInVisitor", $"{entity.Reference} badge {entity.BadgeIssued}");
        return true;
    }

    public async Task<bool> CheckOutAsync(int id, string by)
    {
        var entity = await _db.Visitors.FirstOrDefaultAsync(v => v.Id == id);
        if (entity is null || entity.Status != VisitorStatus.SurSite) return false;
        entity.Status = VisitorStatus.Sorti;
        entity.CheckOutAt = DateTime.UtcNow;
        entity.CheckedOutBy = by;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CheckOutVisitor", $"{entity.Reference}");
        return true;
    }

    public async Task<bool> CancelAsync(int id)
    {
        var entity = await _db.Visitors.FirstOrDefaultAsync(v => v.Id == id);
        if (entity is null || entity.Status == VisitorStatus.SurSite) return false;
        entity.Status = VisitorStatus.Annule;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CancelVisitor", $"{entity.Reference}");
        return true;
    }

    public string NextReference(int year)
    {
        var prefix = $"VIS-{year}-";
        var lastNumber = _db.Visitors
            .IgnoreQueryFilters()
            .Where(v => v.Reference.StartsWith(prefix))
            .AsEnumerable()
            .Select(v => int.TryParse(v.Reference[prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return prefix + (lastNumber + 1).ToString("D4");
    }
}
