using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _db;
    private readonly IClock _clock;
    private readonly IAuditService _audit;

    public DocumentService(ApplicationDbContext db, IClock clock, IAuditService audit)
    {
        _db = db;
        _clock = clock;
        _audit = audit;
    }

    public async Task<IReadOnlyList<SecurityDocument>> GetAllAsync(DocumentCategory? category = null)
    {
        IQueryable<SecurityDocument> q = _db.SecurityDocuments.AsNoTracking();
        if (category is { } c) q = q.Where(d => d.Category == c);
        return await q.OrderBy(d => d.Category).ThenByDescending(d => d.UpdatedAt).ToListAsync();
    }

    public Task<SecurityDocument?> GetByIdAsync(int id)
        => _db.SecurityDocuments.FirstOrDefaultAsync(d => d.Id == id);

    public async Task<SecurityDocument> CreateAsync(SecurityDocument input)
    {
        input.CreatedAt = DateTime.UtcNow;
        input.UpdatedAt = DateTime.UtcNow;
        input.Owner = input.Owner?.Trim() ?? string.Empty;
        input.Description = input.Description?.Trim() ?? string.Empty;
        _db.SecurityDocuments.Add(input);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CreateDocument", $"{input.Title} v{input.Version}");
        return input;
    }

    public async Task<bool> UpdateAsync(SecurityDocument input)
    {
        var existing = await _db.SecurityDocuments.FirstOrDefaultAsync(d => d.Id == input.Id);
        if (existing is null) return false;

        existing.Title = input.Title.Trim();
        existing.Category = input.Category;
        existing.Version = input.Version.Trim();
        existing.Owner = input.Owner?.Trim() ?? string.Empty;
        existing.Description = input.Description?.Trim() ?? string.Empty;
        existing.NextReviewDate = input.NextReviewDate;
        existing.IsConfidential = input.IsConfidential;
        if (!string.IsNullOrEmpty(input.FilePath))
        {
            existing.FilePath = input.FilePath;
            existing.FileName = input.FileName;
            existing.FileSizeBytes = input.FileSizeBytes;
        }
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateDocument", $"{existing.Title} v{existing.Version}");
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, DocumentStatus status)
    {
        var doc = await _db.SecurityDocuments.FirstOrDefaultAsync(d => d.Id == id);
        if (doc is null) return false;
        doc.Status = status;
        doc.UpdatedAt = DateTime.UtcNow;
        if (status == DocumentStatus.EnVigueur && doc.EffectiveDate is null)
            doc.EffectiveDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateDocumentStatus", $"{doc.Title} → {status}");
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var doc = await _db.SecurityDocuments.FirstOrDefaultAsync(d => d.Id == id);
        if (doc is null) return false;
        _db.SecurityDocuments.Remove(doc);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("DeleteDocument", doc.Title);
        return true;
    }

    public async Task<IReadOnlyList<SecurityDocument>> GetDueForReviewAsync(int daysAhead = 30)
    {
        var horizon = _clock.Today.AddDays(daysAhead);
        return await _db.SecurityDocuments.AsNoTracking()
            .Where(d => d.Status == DocumentStatus.EnVigueur
                        && d.NextReviewDate != null && d.NextReviewDate <= horizon)
            .OrderBy(d => d.NextReviewDate)
            .ToListAsync();
    }
}
