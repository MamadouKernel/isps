using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;

namespace IspsDashboard.Services.Interfaces;

public enum AccessPassState { Valide, Expire, Revoque }

public interface IAccessPassService
{
    Task<IReadOnlyList<AccessPass>> SearchAsync(AccessPassType? type, AccessPassCategory? category, string? query);
    Task<AccessPass?> GetByIdAsync(int id);
    Task<AccessPass> CreateAsync(AccessPass input);
    Task<bool> UpdateAsync(AccessPass input, byte[] rowVersion);
    Task<bool> RevokeAsync(int id);
    Task<int> CountActiveAsync();
    Task<int> CountExpiringAsync(int daysAhead = 7);
    Task<int> CountActiveByAsync(AccessPassType? type = null, AccessPassCategory? category = null);
    Task<IReadOnlyList<AccessPass>> GetExpiringSoonAsync(int daysAhead = 30);

    AccessPassState GetState(AccessPass pass);
    int DaysToExpiry(AccessPass pass);
    AccessPassWithCountdown Wrap(AccessPass pass);

    /// <summary>Calcule la date de fin par défaut selon le type et la date d'édition.</summary>
    DateTime ComputeDefaultEndDate(AccessPassType type, DateTime issueDate);

    string NextReference(int year);
}
