using IspsDashboard.Models.Entities;

namespace IspsDashboard.Services.Interfaces;

public interface IHabilitationService
{
    Task<Habilitation> CreateAsync(int agentId, Habilitation input);
    Task<int> UpdateAsync(Habilitation input, byte[] rowVersion);
    Task<Habilitation?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<IReadOnlyList<Habilitation>> GetAllAsync();
    Task<IReadOnlyList<Habilitation>> GetExpiringSoonAsync(int daysAhead = 60);
    Task<IReadOnlyList<Habilitation>> GetExpiredAsync();

    /// <summary>Calcule le nombre de jours avant expiration (négatif si déjà expirée).</summary>
    int DaysToExpiry(Habilitation habilitation);

    /// <summary>"OK", "Bientôt", "Expirée", classification métier.</summary>
    HabilitationStatus ClassifyExpiry(Habilitation habilitation);
}

public enum HabilitationStatus
{
    Valide = 0,
    Bientot = 1,   // < 60 jours
    Urgent = 2,    // < 30 jours
    Expiree = 3
}
