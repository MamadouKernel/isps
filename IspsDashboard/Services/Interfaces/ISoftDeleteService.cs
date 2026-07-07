using IspsDashboard.Models.Entities;

namespace IspsDashboard.Services.Interfaces;

public interface ISoftDeleteService
{
    Task<bool> DeleteAsync<T>(int id, string? userId) where T : class, ISoftDeletable;
    Task<bool> RestoreAsync<T>(int id) where T : class, ISoftDeletable;
    Task<IReadOnlyList<T>> GetDeletedAsync<T>() where T : class, ISoftDeletable;
    Task<int> CountDeletedAsync<T>() where T : class, ISoftDeletable;

    /// <summary>Supprime définitivement les entités soft-deleted depuis plus de <paramref name="olderThan"/>.</summary>
    Task<int> PurgeOlderThanAsync<T>(TimeSpan olderThan) where T : class, ISoftDeletable;
}
