using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Services.Interfaces;

public interface IDocumentService
{
    Task<IReadOnlyList<SecurityDocument>> GetAllAsync(DocumentCategory? category = null);
    Task<SecurityDocument?> GetByIdAsync(int id);
    Task<SecurityDocument> CreateAsync(SecurityDocument input);
    Task<bool> UpdateAsync(SecurityDocument input);
    Task<bool> UpdateStatusAsync(int id, DocumentStatus status);
    Task<bool> DeleteAsync(int id);
    Task<IReadOnlyList<SecurityDocument>> GetDueForReviewAsync(int daysAhead = 30);
}
