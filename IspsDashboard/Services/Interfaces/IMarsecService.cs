using IspsDashboard.Models.Entities;

namespace IspsDashboard.Services.Interfaces;

public interface IMarsecService
{
    Task<int> GetCurrentLevelAsync();
    Task<MarsecLevelChange> RequestChangeAsync(int newLevel, string reason, string decisionSource,
        string decidedBy, string? decidedById);
    Task<IReadOnlyList<MarsecLevelChange>> GetHistoryAsync(int take = 50);
    Task<bool> CompleteChecklistItemAsync(int itemId, string completedBy, string? notes);
    IReadOnlyList<string> GetChecklistTemplate(int targetLevel);
}
