using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class NonConformityService : INonConformityService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public NonConformityService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<IReadOnlyList<NonConformity>> GetOpenAsync()
        => await _db.NonConformities.AsNoTracking()
            .Where(n => n.Status != NonConformityStatus.Levee && n.Status != NonConformityStatus.Rejetee)
            .OrderBy(n => n.DueDate)
            .ToListAsync();

    public async Task<IReadOnlyList<NonConformity>> SearchAsync(NonConformityStatus? status)
    {
        IQueryable<NonConformity> q = _db.NonConformities.AsNoTracking();
        if (status is { } s) q = q.Where(n => n.Status == s);
        return await q.OrderByDescending(n => n.IdentifiedAt).Take(500).ToListAsync();
    }

    public Task<NonConformity?> GetByIdAsync(int id)
        => _db.NonConformities.FirstOrDefaultAsync(n => n.Id == id);

    public async Task<NonConformity> CreateAsync(NonConformity input)
    {
        // Entité neuve à partir des seuls champs légitimes (voir AuditCampaignService.CreateAsync) :
        // ClosedAt/ClosureEvidence ne doivent jamais venir du formulaire de création.
        var entity = new NonConformity
        {
            Title = input.Title.Trim(),
            Source = input.Source,
            Status = NonConformityStatus.Identifiee,
            Description = input.Description.Trim(),
            CorrectiveAction = input.CorrectiveAction?.Trim() ?? string.Empty,
            Owner = input.Owner?.Trim() ?? string.Empty,
            IdentifiedAt = input.IdentifiedAt,
            DueDate = input.DueDate,
            CreatedAt = DateTime.UtcNow
        };
        _db.NonConformities.Add(entity);
        await ReferenceGenerator.SaveWithUniqueReferenceAsync(_db, r => entity.Reference = r, () => NextReference(entity.IdentifiedAt.Year));
        await _audit.LogAsync("CreateNonConformity", $"{entity.Reference} — {entity.Title}");
        return entity;
    }

    public async Task<bool> UpdateAsync(NonConformity input, byte[] rowVersion)
    {
        var e = await _db.NonConformities.FirstOrDefaultAsync(n => n.Id == input.Id);
        if (e is null) return false;
        _db.Entry(e).Property(nameof(NonConformity.RowVersion)).OriginalValue = rowVersion;
        e.Title = input.Title.Trim();
        e.Source = input.Source;
        e.Description = input.Description?.Trim() ?? string.Empty;
        e.CorrectiveAction = input.CorrectiveAction?.Trim() ?? string.Empty;
        e.Owner = input.Owner?.Trim() ?? string.Empty;
        e.IdentifiedAt = input.IdentifiedAt;
        e.DueDate = input.DueDate;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateNonConformity", e.Reference);
        return true;
    }

    public async Task<bool> CloseAsync(int id, string evidence)
    {
        var entity = await _db.NonConformities.FirstOrDefaultAsync(n => n.Id == id);
        if (entity is null) return false;
        entity.Status = NonConformityStatus.Levee;
        entity.ClosedAt = DateTime.UtcNow;
        entity.ClosureEvidence = evidence?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CloseNonConformity", $"{entity.Reference}");
        return true;
    }

    public string NextReference(int year)
    {
        var prefix = $"NC-{year}-";
        var lastNumber = _db.NonConformities
            .IgnoreQueryFilters()
            .Where(n => n.Reference.StartsWith(prefix))
            .AsEnumerable()
            .Select(n => int.TryParse(n.Reference[prefix.Length..], out var x) ? x : 0)
            .DefaultIfEmpty(0)
            .Max();
        return prefix + (lastNumber + 1).ToString("D4");
    }
}
