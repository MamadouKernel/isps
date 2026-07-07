using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class SoftDeleteService : ISoftDeleteService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public SoftDeleteService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<bool> DeleteAsync<T>(int id, string? userId) where T : class, ISoftDeletable
    {
        var entity = await _db.Set<T>().FindAsync(id);
        if (entity is null || entity.IsDeleted) return false;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedById = userId;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("SoftDelete", $"{typeof(T).Name} #{id}");
        return true;
    }

    public async Task<bool> RestoreAsync<T>(int id) where T : class, ISoftDeletable
    {
        var entity = await _db.Set<T>().IgnoreQueryFilters().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        if (entity is null || !entity.IsDeleted) return false;
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.DeletedById = null;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Restore", $"{typeof(T).Name} #{id}");
        return true;
    }

    public async Task<IReadOnlyList<T>> GetDeletedAsync<T>() where T : class, ISoftDeletable
        => await _db.Set<T>().IgnoreQueryFilters().Where(e => e.IsDeleted)
            .OrderByDescending(e => e.DeletedAt).Take(200).ToListAsync();

    public Task<int> CountDeletedAsync<T>() where T : class, ISoftDeletable
        => _db.Set<T>().IgnoreQueryFilters().CountAsync(e => e.IsDeleted);

    public async Task<int> PurgeOlderThanAsync<T>(TimeSpan olderThan) where T : class, ISoftDeletable
    {
        var cutoff = DateTime.UtcNow - olderThan;
        var stale = await _db.Set<T>().IgnoreQueryFilters()
            .Where(e => e.IsDeleted && e.DeletedAt != null && e.DeletedAt < cutoff)
            .ToListAsync();
        if (stale.Count == 0) return 0;
        _db.Set<T>().RemoveRange(stale);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("PurgeSoftDeleted", $"{typeof(T).Name} x{stale.Count} (> {olderThan.TotalDays:0} j)");
        return stale.Count;
    }
}
