using IspsDashboard.Models.Entities;

namespace IspsDashboard.Services.Interfaces;

public interface IAuditCampaignService
{
    Task<IReadOnlyList<SecurityAudit>> GetAllAsync();
    Task<SecurityAudit?> GetByIdAsync(int id);
    Task<SecurityAudit> CreateAsync(SecurityAudit input, bool seedChecklist);
    Task<bool> UpdateHeaderAsync(SecurityAudit input);
    Task<bool> UpdateFindingAsync(int findingId, Models.Enums.FindingResult result, string observation);
    Task<bool> CloseAsync(int id, string conclusion);
    string NextReference(int year);
    IReadOnlyList<string> GetIspsChecklistTemplate();
}
