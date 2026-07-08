using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class VehicleAccessService : IVehicleAccessService
{
    private readonly ApplicationDbContext _db;
    private readonly IClock _clock;
    private readonly IAuditService _audit;

    public VehicleAccessService(ApplicationDbContext db, IClock clock, IAuditService audit)
    {
        _db = db;
        _clock = clock;
        _audit = audit;
    }

    public async Task<IReadOnlyList<VehicleAccess>> SearchAsync(AccessDirection? direction, DateTime? from, DateTime? to, string? query)
    {
        IQueryable<VehicleAccess> q = _db.VehicleAccesses.AsNoTracking();
        if (direction is { } d) q = q.Where(v => v.Direction == d);
        if (from is { } f) q = q.Where(v => v.OccurredAt >= f);
        if (to is { } t) q = q.Where(v => v.OccurredAt <= t.AddDays(1));
        if (!string.IsNullOrWhiteSpace(query))
        {
            var s = query.Trim();
            q = q.Where(v => EF.Functions.Like(v.Plate, $"%{s}%")
                          || EF.Functions.Like(v.ContainerNumber, $"%{s}%")
                          || EF.Functions.Like(v.DriverName, $"%{s}%")
                          || EF.Functions.Like(v.Carrier, $"%{s}%"));
        }
        return await q.OrderByDescending(v => v.OccurredAt).Take(500).ToListAsync();
    }

    public async Task<IReadOnlyList<VehicleAccess>> GetTodayAsync()
    {
        var today = _clock.Today;
        var tomorrow = today.AddDays(1);
        return await _db.VehicleAccesses.AsNoTracking()
            .Where(v => v.OccurredAt >= today && v.OccurredAt < tomorrow)
            .OrderByDescending(v => v.OccurredAt)
            .ToListAsync();
    }

    public Task<int> CountTodayAsync()
    {
        var today = _clock.Today;
        var tomorrow = today.AddDays(1);
        return _db.VehicleAccesses.CountAsync(v => v.OccurredAt >= today && v.OccurredAt < tomorrow);
    }

    public async Task<VehicleAccess> CreateAsync(VehicleAccess input)
    {
        // Entité neuve à partir des seuls champs légitimes (voir AuditCampaignService.CreateAsync).
        var occurredAt = input.OccurredAt == default ? DateTime.UtcNow : input.OccurredAt;
        var entity = new VehicleAccess
        {
            Plate = input.Plate.Trim(),
            Type = input.Type,
            Direction = input.Direction,
            Result = input.Result,
            DriverName = input.DriverName?.Trim() ?? string.Empty,
            DriverIdNumber = input.DriverIdNumber?.Trim() ?? string.Empty,
            Carrier = input.Carrier?.Trim() ?? string.Empty,
            ContainerNumber = input.ContainerNumber?.Trim() ?? string.Empty,
            SealNumber = input.SealNumber?.Trim() ?? string.Empty,
            SealVerified = input.SealVerified,
            BookingReference = input.BookingReference?.Trim() ?? string.Empty,
            Searched = input.Searched,
            Controller = input.Controller?.Trim() ?? string.Empty,
            Gate = input.Gate?.Trim() ?? string.Empty,
            OccurredAt = occurredAt,
            Notes = input.Notes?.Trim() ?? string.Empty
        };
        _db.VehicleAccesses.Add(entity);
        await ReferenceGenerator.SaveWithUniqueReferenceAsync(_db, r => entity.Reference = r, () => NextReference(occurredAt.Year));
        await _audit.LogAsync("VehicleAccess", $"{entity.Reference} — {entity.Plate} ({entity.Direction}, {entity.Result})");
        return entity;
    }

    public Task<VehicleAccess?> GetByIdAsync(int id)
        => _db.VehicleAccesses.FirstOrDefaultAsync(v => v.Id == id);

    public async Task<bool> UpdateAsync(VehicleAccess input)
    {
        var e = await _db.VehicleAccesses.FirstOrDefaultAsync(v => v.Id == input.Id);
        if (e is null) return false;
        e.Plate = input.Plate.Trim();
        e.Type = input.Type;
        e.Direction = input.Direction;
        e.Result = input.Result;
        e.OccurredAt = input.OccurredAt;
        e.Gate = input.Gate?.Trim() ?? string.Empty;
        e.DriverName = input.DriverName?.Trim() ?? string.Empty;
        e.DriverIdNumber = input.DriverIdNumber?.Trim() ?? string.Empty;
        e.Carrier = input.Carrier?.Trim() ?? string.Empty;
        e.BookingReference = input.BookingReference?.Trim() ?? string.Empty;
        e.ContainerNumber = input.ContainerNumber?.Trim() ?? string.Empty;
        e.SealNumber = input.SealNumber?.Trim() ?? string.Empty;
        e.SealVerified = input.SealVerified;
        e.Searched = input.Searched;
        e.Notes = input.Notes?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateVehicleAccess", e.Reference);
        return true;
    }

    public string NextReference(int year)
    {
        var prefix = $"ACC-{year}-";
        var last = _db.VehicleAccesses
            .IgnoreQueryFilters()
            .Where(v => v.Reference.StartsWith(prefix))
            .AsEnumerable()
            .Select(v => int.TryParse(v.Reference[prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return prefix + (last + 1).ToString("D4");
    }
}
