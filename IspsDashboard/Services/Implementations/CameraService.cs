using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class CameraService : ICameraService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public CameraService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<IReadOnlyList<Camera>> GetAllAsync()
        => await _db.Cameras.AsNoTracking()
            .Include(c => c.MaintenanceHistory.OrderByDescending(m => m.PerformedAt).Take(3))
            .OrderBy(c => c.Code)
            .ToListAsync();

    public Task<Camera?> GetByIdAsync(int id)
        => _db.Cameras.Include(c => c.MaintenanceHistory).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Camera> CreateAsync(Camera input)
    {
        input.CreatedAt = DateTime.UtcNow;
        input.Zone = input.Zone?.Trim() ?? string.Empty;
        input.Model = input.Model?.Trim() ?? string.Empty;
        input.IpAddress = input.IpAddress?.Trim() ?? string.Empty;
        input.Notes = input.Notes?.Trim() ?? string.Empty;
        _db.Cameras.Add(input);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CreateCamera", $"{input.Code} — {input.Label}");
        return input;
    }

    public async Task<bool> UpdateAsync(Camera input)
    {
        var existing = await _db.Cameras.FirstOrDefaultAsync(c => c.Id == input.Id);
        if (existing is null) return false;
        existing.Label = input.Label.Trim();
        existing.Zone = input.Zone?.Trim() ?? string.Empty;
        existing.Type = input.Type;
        existing.Model = input.Model?.Trim() ?? string.Empty;
        existing.IpAddress = input.IpAddress?.Trim() ?? string.Empty;
        existing.Notes = input.Notes?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateCamera", existing.Code);
        return true;
    }

    public async Task<bool> ChangeStatusAsync(int id, CameraStatus newStatus, string by, string? notes)
    {
        var cam = await _db.Cameras.FirstOrDefaultAsync(c => c.Id == id);
        if (cam is null) return false;
        var old = cam.Status;
        cam.Status = newStatus;
        cam.LastCheckedAt = DateTime.UtcNow;
        cam.LastCheckedBy = by?.Trim() ?? string.Empty;
        _db.CameraMaintenances.Add(new CameraMaintenance
        {
            CameraId = id,
            Action = $"Changement de statut : {old} → {newStatus}",
            PerformedBy = by?.Trim() ?? string.Empty,
            Notes = notes?.Trim(),
            ResultingStatus = newStatus
        });
        await _db.SaveChangesAsync();
        await _audit.LogAsync("ChangeCameraStatus", $"{cam.Code}: {old} → {newStatus}");
        return true;
    }

    public async Task<CameraMaintenance> LogMaintenanceAsync(int cameraId, CameraMaintenance entry)
    {
        entry.CameraId = cameraId;
        entry.PerformedAt = DateTime.UtcNow;
        entry.PerformedBy = entry.PerformedBy?.Trim() ?? string.Empty;
        _db.CameraMaintenances.Add(entry);

        var cam = await _db.Cameras.FirstOrDefaultAsync(c => c.Id == cameraId);
        if (cam is not null)
        {
            cam.Status = entry.ResultingStatus;
            cam.LastCheckedAt = DateTime.UtcNow;
            cam.LastCheckedBy = entry.PerformedBy;
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CameraMaintenance", $"{cam?.Code}: {entry.Action}");
        return entry;
    }

    public Task<int> CountAvailableAsync()
        => _db.Cameras.CountAsync(c => c.Status == CameraStatus.Operationnelle);

    public async Task<double> AvailabilityPercentAsync()
    {
        var total = await _db.Cameras.CountAsync();
        if (total == 0) return 100;
        var ok = await CountAvailableAsync();
        return Math.Round((double)ok / total * 100, 1);
    }
}
