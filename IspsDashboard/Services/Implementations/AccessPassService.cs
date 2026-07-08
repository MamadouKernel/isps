using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class AccessPassService : IAccessPassService
{
    private readonly ApplicationDbContext _db;
    private readonly IClock _clock;
    private readonly IAuditService _audit;

    public AccessPassService(ApplicationDbContext db, IClock clock, IAuditService audit)
    {
        _db = db;
        _clock = clock;
        _audit = audit;
    }

    public async Task<IReadOnlyList<AccessPass>> SearchAsync(AccessPassType? type, AccessPassCategory? category, string? query)
    {
        IQueryable<AccessPass> q = _db.AccessPasses.AsNoTracking();
        if (type is { } t) q = q.Where(p => p.Type == t);
        if (category is { } c) q = q.Where(p => p.Category == c);
        if (!string.IsNullOrWhiteSpace(query))
        {
            var s = query.Trim();
            q = q.Where(p => EF.Functions.Like(p.LastName, $"%{s}%")
                          || EF.Functions.Like(p.FirstName, $"%{s}%")
                          || EF.Functions.Like(p.Matricule, $"%{s}%")
                          || EF.Functions.Like(p.Plate, $"%{s}%")
                          || EF.Functions.Like(p.Reference, $"%{s}%"));
        }
        return await q.OrderByDescending(p => p.IssueDate).Take(500).ToListAsync();
    }

    public Task<AccessPass?> GetByIdAsync(int id)
        => _db.AccessPasses.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<AccessPass> CreateAsync(AccessPass input)
    {
        // Entité neuve à partir des seuls champs légitimes (voir AuditCampaignService.CreateAsync
        // pour le pourquoi) : Revoked/IsDeleted ne doivent jamais venir du formulaire de création.
        var entity = new AccessPass
        {
            Type = input.Type,
            Category = input.Category,
            LastName = input.LastName.Trim(),
            FirstName = input.FirstName?.Trim() ?? string.Empty,
            Contact = input.Contact?.Trim() ?? string.Empty,
            Matricule = input.Matricule?.Trim() ?? string.Empty,
            Email = input.Email?.Trim() ?? string.Empty,
            Plate = input.Plate?.Trim() ?? string.Empty,
            Company = input.Company?.Trim() ?? string.Empty,
            IssueDate = input.IssueDate,
            EndDate = input.EndDate,
            IssuedBy = input.IssuedBy?.Trim() ?? string.Empty,
            Notes = input.Notes?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };
        _db.AccessPasses.Add(entity);
        await ReferenceGenerator.SaveWithUniqueReferenceAsync(_db, r => entity.Reference = r, () => NextReference(entity.IssueDate.Year));
        await _audit.LogAsync("CreateAccessPass", $"{entity.Reference} — {entity.FullName} ({entity.Type})");
        return entity;
    }

    public async Task<bool> UpdateAsync(AccessPass input, byte[] rowVersion)
    {
        var existing = await _db.AccessPasses.FirstOrDefaultAsync(p => p.Id == input.Id);
        if (existing is null) return false;
        _db.Entry(existing).Property(nameof(AccessPass.RowVersion)).OriginalValue = rowVersion;

        existing.Type = input.Type;
        existing.Category = input.Category;
        existing.LastName = input.LastName.Trim();
        existing.FirstName = input.FirstName?.Trim() ?? string.Empty;
        existing.Contact = input.Contact?.Trim() ?? string.Empty;
        existing.Matricule = input.Matricule?.Trim() ?? string.Empty;
        existing.Email = input.Email?.Trim() ?? string.Empty;
        existing.Plate = input.Plate?.Trim() ?? string.Empty;
        existing.Company = input.Company?.Trim() ?? string.Empty;
        existing.IssueDate = input.IssueDate;
        existing.EndDate = input.EndDate;
        existing.Notes = input.Notes?.Trim() ?? string.Empty;

        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateAccessPass", existing.Reference);
        return true;
    }

    public async Task<bool> RevokeAsync(int id)
    {
        var pass = await _db.AccessPasses.FirstOrDefaultAsync(p => p.Id == id);
        if (pass is null) return false;
        pass.Revoked = true;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("RevokeAccessPass", pass.Reference);
        return true;
    }

    public Task<int> CountActiveAsync()
    {
        var today = _clock.Today;
        return _db.AccessPasses.CountAsync(p => !p.Revoked && p.EndDate >= today && p.IssueDate <= today);
    }

    public Task<int> CountExpiringAsync(int daysAhead = 7)
    {
        var today = _clock.Today;
        var horizon = today.AddDays(daysAhead);
        return _db.AccessPasses.CountAsync(p => !p.Revoked && p.EndDate >= today && p.EndDate <= horizon);
    }

    public Task<int> CountActiveByAsync(AccessPassType? type = null, AccessPassCategory? category = null)
    {
        var today = _clock.Today;
        IQueryable<AccessPass> q = _db.AccessPasses.Where(p => !p.Revoked && p.EndDate >= today && p.IssueDate <= today);
        if (type is { } t) q = q.Where(p => p.Type == t);
        if (category is { } c) q = q.Where(p => p.Category == c);
        return q.CountAsync();
    }

    public async Task<IReadOnlyList<AccessPass>> GetExpiringSoonAsync(int daysAhead = 30)
    {
        var today = _clock.Today;
        var horizon = today.AddDays(daysAhead);
        return await _db.AccessPasses.AsNoTracking()
            .Where(p => !p.Revoked && p.EndDate >= today && p.EndDate <= horizon)
            .OrderBy(p => p.EndDate)
            .ToListAsync();
    }

    public AccessPassState GetState(AccessPass pass)
    {
        if (pass.Revoked) return AccessPassState.Revoque;
        return pass.EndDate.Date < _clock.Today ? AccessPassState.Expire : AccessPassState.Valide;
    }

    public int DaysToExpiry(AccessPass pass) => (int)(pass.EndDate.Date - _clock.Today).TotalDays;

    public AccessPassWithCountdown Wrap(AccessPass pass)
    {
        var days = DaysToExpiry(pass);
        var color = days <= 7 ? KpiCardColor.Rouge : days <= 30 ? KpiCardColor.Ambre : KpiCardColor.Vert;
        var label = days < 0 ? $"Expiré depuis {Math.Abs(days)} j" : days == 0 ? "Aujourd'hui" : $"J-{days}";
        return new AccessPassWithCountdown
        {
            Pass = pass,
            DaysRemaining = days,
            Color = color,
            CountdownLabel = label
        };
    }

    public DateTime ComputeDefaultEndDate(AccessPassType type, DateTime issueDate) => type switch
    {
        AccessPassType.Journalier => issueDate.Date.AddDays(1).AddSeconds(-1),
        AccessPassType.Trimestriel => issueDate.Date.AddMonths(3),
        AccessPassType.Annuel => issueDate.Date.AddYears(1),
        _ => issueDate.Date.AddDays(1)
    };

    public string NextReference(int year)
    {
        var prefix = $"LP-{year}-";
        var last = _db.AccessPasses
            .IgnoreQueryFilters()
            .Where(p => p.Reference.StartsWith(prefix))
            .AsEnumerable()
            .Select(p => int.TryParse(p.Reference[prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return prefix + (last + 1).ToString("D4");
    }
}
