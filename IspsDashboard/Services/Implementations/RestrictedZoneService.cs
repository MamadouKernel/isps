using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class RestrictedZoneService : IRestrictedZoneService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public RestrictedZoneService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<IReadOnlyList<RestrictedZone>> GetAllAsync()
        => await _db.RestrictedZones.AsNoTracking()
            .OrderByDescending(z => z.AccessLevel).ThenBy(z => z.Code)
            .ToListAsync();

    public Task<RestrictedZone?> GetByIdAsync(int id)
        => _db.RestrictedZones.FirstOrDefaultAsync(z => z.Id == id);

    public async Task<RestrictedZone> CreateAsync(RestrictedZone input)
    {
        input.CreatedAt = DateTime.UtcNow;
        input.UpdatedAt = DateTime.UtcNow;
        input.Description = input.Description?.Trim() ?? string.Empty;
        input.ProtectionMeasures = input.ProtectionMeasures?.Trim() ?? string.Empty;
        input.AuthorizedPersonnel = input.AuthorizedPersonnel?.Trim() ?? string.Empty;
        input.ZoneManager = input.ZoneManager?.Trim() ?? string.Empty;
        _db.RestrictedZones.Add(input);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CreateRestrictedZone", $"{input.Code} — {input.Name}");
        return input;
    }

    public async Task<bool> UpdateAsync(RestrictedZone input, byte[] rowVersion)
    {
        var existing = await _db.RestrictedZones.FirstOrDefaultAsync(z => z.Id == input.Id);
        if (existing is null) return false;
        _db.Entry(existing).Property(nameof(RestrictedZone.RowVersion)).OriginalValue = rowVersion;

        existing.Name = input.Name.Trim();
        existing.AccessLevel = input.AccessLevel;
        existing.Description = input.Description?.Trim() ?? string.Empty;
        existing.ProtectionMeasures = input.ProtectionMeasures?.Trim() ?? string.Empty;
        existing.AuthorizedPersonnel = input.AuthorizedPersonnel?.Trim() ?? string.Empty;
        existing.ZoneManager = input.ZoneManager?.Trim() ?? string.Empty;
        existing.RequiresEscort = input.RequiresEscort;
        existing.RequiresClearance = input.RequiresClearance;
        existing.CctvMonitored = input.CctvMonitored;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateRestrictedZone", existing.Code);
        return true;
    }

    public async Task<bool> SetStatusAsync(int id, ZoneStatus status)
    {
        var zone = await _db.RestrictedZones.FirstOrDefaultAsync(z => z.Id == id);
        if (zone is null) return false;
        zone.Status = status;
        zone.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("SetZoneStatus", $"{zone.Code} → {status}");
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var zone = await _db.RestrictedZones.FirstOrDefaultAsync(z => z.Id == id);
        if (zone is null) return false;
        _db.RestrictedZones.Remove(zone);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("DeleteRestrictedZone", zone.Code);
        return true;
    }
}
