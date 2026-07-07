using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Services.Interfaces;

public interface IBriefingService
{
    Task<IReadOnlyList<ShiftBriefing>> GetRecentAsync(int take = 30);
    Task<ShiftBriefing?> GetByIdAsync(int id);
    Task<ShiftBriefing?> GetLatestAsync();
    Task<ShiftBriefing> CreateAsync(ShiftBriefing input);
    Task<bool> UpdateAsync(ShiftBriefing input, byte[] rowVersion);
    Task<bool> AcknowledgeAsync(int id, string by);
    ShiftSlot DetermineCurrentSlot(DateTime now);
}
