using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class IncidentService : IIncidentService
{
    private readonly ApplicationDbContext _db;
    private readonly IClock _clock;
    private readonly IAuditService _audit;

    public IncidentService(ApplicationDbContext db, IClock clock, IAuditService audit)
    {
        _db = db;
        _clock = clock;
        _audit = audit;
    }

    public async Task<IReadOnlyList<Incident>> SearchAsync(IncidentSearchCriteria criteria)
    {
        IQueryable<Incident> query = _db.Incidents.AsNoTracking().Include(i => i.Attachments);

        if (criteria.Status is { } status) query = query.Where(i => i.Status == status);
        if (criteria.Severity is { } severity) query = query.Where(i => i.Severity == severity);
        if (criteria.Category is { } category) query = query.Where(i => i.Category == category);
        if (criteria.FromDate is { } from) query = query.Where(i => i.OccurredAt >= from);
        if (criteria.ToDate is { } to) query = query.Where(i => i.OccurredAt <= to.AddDays(1));
        if (!string.IsNullOrWhiteSpace(criteria.Query))
        {
            var q = criteria.Query.Trim();
            query = query.Where(i =>
                EF.Functions.Like(i.Title, $"%{q}%") ||
                EF.Functions.Like(i.Reference, $"%{q}%") ||
                EF.Functions.Like(i.Description, $"%{q}%") ||
                EF.Functions.Like(i.Zone, $"%{q}%"));
        }

        return await query.OrderByDescending(i => i.OccurredAt).Take(500).ToListAsync();
    }

    public Task<Incident?> GetByIdAsync(int id)
        => _db.Incidents.Include(i => i.Attachments).FirstOrDefaultAsync(i => i.Id == id);

    public async Task<Incident> CreateAsync(IncidentEditViewModel input, string createdById)
    {
        var year = input.OccurredAt.Year;
        var entity = new Incident
        {
            Title = input.Title.Trim(),
            Category = input.Category,
            Severity = input.Severity,
            Status = IncidentStatus.Ouvert,
            OccurredAt = input.OccurredAt,
            Zone = input.Zone?.Trim() ?? string.Empty,
            ReportedBy = input.ReportedBy?.Trim() ?? string.Empty,
            Investigator = input.Investigator?.Trim() ?? string.Empty,
            Description = input.Description.Trim(),
            ActionsTaken = input.ActionsTaken?.Trim() ?? string.Empty,
            LessonsLearned = input.LessonsLearned?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedById = createdById
        };
        _db.Incidents.Add(entity);
        await ReferenceGenerator.SaveWithUniqueReferenceAsync(_db, r => entity.Reference = r, () => NextReference(year));
        await _audit.LogAsync("CreateIncident", $"{entity.Reference} — {entity.Title}");
        return entity;
    }

    public async Task<bool> UpdateAsync(int id, IncidentEditViewModel input, byte[] rowVersion)
    {
        var entity = await _db.Incidents.FirstOrDefaultAsync(i => i.Id == id);
        if (entity is null) return false;

        _db.Entry(entity).Property(nameof(Incident.RowVersion)).OriginalValue = rowVersion;

        entity.Title = input.Title.Trim();
        entity.Category = input.Category;
        entity.Severity = input.Severity;
        entity.OccurredAt = input.OccurredAt;
        entity.Zone = input.Zone?.Trim() ?? string.Empty;
        entity.ReportedBy = input.ReportedBy?.Trim() ?? string.Empty;
        entity.Investigator = input.Investigator?.Trim() ?? string.Empty;
        entity.Description = input.Description.Trim();
        entity.ActionsTaken = input.ActionsTaken?.Trim() ?? string.Empty;
        entity.LessonsLearned = input.LessonsLearned?.Trim() ?? string.Empty;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateIncident", $"{entity.Reference} — modifié");
        return true;
    }

    public async Task<bool> ChangeStatusAsync(int id, IncidentStatus newStatus, string? userId)
    {
        var entity = await _db.Incidents.FirstOrDefaultAsync(i => i.Id == id);
        if (entity is null) return false;
        if (entity.Status == newStatus) return true;

        entity.Status = newStatus;
        entity.UpdatedAt = DateTime.UtcNow;
        if (newStatus == IncidentStatus.Clos)
        {
            entity.ClosedAt = DateTime.UtcNow;
            entity.ClosedById = userId;
        }
        else
        {
            // Réouverture (ou tout autre statut) : effacer une éventuelle date de clôture
            // obsolète, sinon l'incident continue d'afficher "Clos le ..." après réouverture.
            entity.ClosedAt = null;
            entity.ClosedById = null;
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync("ChangeIncidentStatus", $"{entity.Reference} → {newStatus}");
        return true;
    }

    public Task<int> CountByStatusAsync(IncidentStatus status)
        => _db.Incidents.CountAsync(i => i.Status == status);

    public async Task<IReadOnlyList<Incident>> GetRecentAsync(int take = 5)
        => await _db.Incidents.AsNoTracking()
                   .OrderByDescending(i => i.OccurredAt)
                   .Take(take)
                   .ToListAsync();

    public string NextReference(int yearReference)
    {
        var prefix = $"INC-{yearReference}-";
        var lastNumber = _db.Incidents
            .IgnoreQueryFilters()
            .Where(i => i.Reference.StartsWith(prefix))
            .AsEnumerable()
            .Select(i => int.TryParse(i.Reference[prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return prefix + (lastNumber + 1).ToString("D4");
    }
}
