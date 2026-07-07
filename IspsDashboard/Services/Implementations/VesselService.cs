using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class VesselService : IVesselService
{
    private readonly ApplicationDbContext _db;
    private readonly IClock _clock;
    private readonly IAuditService _audit;

    public VesselService(ApplicationDbContext db, IClock clock, IAuditService audit)
    {
        _db = db;
        _clock = clock;
        _audit = audit;
    }

    public async Task<IReadOnlyList<VesselCall>> GetUpcomingAsync(int daysAhead = 30)
    {
        var horizon = _clock.Today.AddDays(daysAhead);
        return await _db.VesselCalls.AsNoTracking()
            .Where(v => v.Eta >= _clock.Today && v.Eta <= horizon
                        && v.Status != VesselCallStatus.Parti && v.Status != VesselCallStatus.Annule)
            .OrderBy(v => v.Eta)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<VesselCall>> SearchAsync(VesselCallStatus? status, DateTime? from, DateTime? to)
    {
        IQueryable<VesselCall> q = _db.VesselCalls.AsNoTracking().Include(v => v.Declarations);
        if (status is { } s) q = q.Where(v => v.Status == s);
        if (from is { } f) q = q.Where(v => v.Eta >= f);
        if (to is { } t) q = q.Where(v => v.Eta <= t.AddDays(1));
        return await q.OrderByDescending(v => v.Eta).Take(500).ToListAsync();
    }

    public Task<VesselCall?> GetByIdAsync(int id)
        => _db.VesselCalls.Include(v => v.Declarations).FirstOrDefaultAsync(v => v.Id == id);

    public async Task<VesselCall> CreateAsync(VesselCall input)
    {
        input.CreatedAt = DateTime.UtcNow;
        input.ImoNumber = input.ImoNumber?.Trim() ?? string.Empty;
        input.CallSign = input.CallSign?.Trim() ?? string.Empty;
        input.Flag = input.Flag?.Trim() ?? string.Empty;
        input.Operator = input.Operator?.Trim() ?? string.Empty;
        input.Cso = input.Cso?.Trim() ?? string.Empty;
        input.Sso = input.Sso?.Trim() ?? string.Empty;
        input.Berth = input.Berth?.Trim() ?? string.Empty;
        input.SecurityNotes = input.SecurityNotes?.Trim() ?? string.Empty;
        input.LastTenPorts = input.LastTenPorts?.Trim() ?? string.Empty;
        _db.VesselCalls.Add(input);
        await ReferenceGenerator.SaveWithUniqueReferenceAsync(_db, r => input.Reference = r, () => NextVesselReference(input.Eta.Year));
        await _audit.LogAsync("CreateVesselCall", $"{input.Reference} — {input.VesselName} (IMO {input.ImoNumber})");
        return input;
    }

    public async Task<bool> UpdateAsync(VesselCall input, byte[] rowVersion)
    {
        var existing = await _db.VesselCalls.FirstOrDefaultAsync(v => v.Id == input.Id);
        if (existing is null) return false;
        _db.Entry(existing).Property(nameof(VesselCall.RowVersion)).OriginalValue = rowVersion;

        existing.VesselName = input.VesselName.Trim();
        existing.ImoNumber = input.ImoNumber?.Trim() ?? string.Empty;
        existing.CallSign = input.CallSign?.Trim() ?? string.Empty;
        existing.Flag = input.Flag?.Trim() ?? string.Empty;
        existing.Operator = input.Operator?.Trim() ?? string.Empty;
        existing.Cso = input.Cso?.Trim() ?? string.Empty;
        existing.Sso = input.Sso?.Trim() ?? string.Empty;
        existing.ShipIspsLevel = Math.Clamp(input.ShipIspsLevel, 1, 3);
        existing.Eta = input.Eta;
        existing.Etd = input.Etd;
        existing.Berth = input.Berth?.Trim() ?? string.Empty;
        existing.SecurityNotes = input.SecurityNotes?.Trim() ?? string.Empty;
        existing.LastTenPorts = input.LastTenPorts?.Trim() ?? string.Empty;
        existing.CrewCount = input.CrewCount;

        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateVesselCall", existing.Reference);
        return true;
    }

    public async Task<bool> ChangeStatusAsync(int id, VesselCallStatus newStatus)
    {
        var entity = await _db.VesselCalls.FirstOrDefaultAsync(v => v.Id == id);
        if (entity is null) return false;
        entity.Status = newStatus;
        if (newStatus == VesselCallStatus.AQuai && entity.ActualArrival is null)
            entity.ActualArrival = DateTime.UtcNow;
        if (newStatus == VesselCallStatus.Parti && entity.ActualDeparture is null)
            entity.ActualDeparture = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("ChangeVesselStatus", $"{entity.Reference} → {newStatus}");
        return true;
    }

    public async Task<DeclarationOfSecurity> CreateDosAsync(int vesselCallId, DeclarationOfSecurity input)
    {
        input.VesselCallId = vesselCallId;
        input.Status = DosStatus.Brouillon;
        input.CreatedAt = DateTime.UtcNow;
        input.AgreedMeasures = input.AgreedMeasures?.Trim() ?? string.Empty;
        _db.DeclarationsOfSecurity.Add(input);
        await ReferenceGenerator.SaveWithUniqueReferenceAsync(_db, r => input.Reference = r, () => NextDosReference(input.EffectiveFrom.Year));
        await _audit.LogAsync("CreateDoS", $"{input.Reference} pour escale #{vesselCallId}");
        return input;
    }

    public async Task<bool> SignDosAsync(int dosId, string pfsoName, string shipName)
    {
        var dos = await _db.DeclarationsOfSecurity.FirstOrDefaultAsync(d => d.Id == dosId);
        if (dos is null) return false;
        dos.SignedByPfso = pfsoName.Trim();
        dos.SignedByShip = shipName.Trim();
        dos.SignedAt = DateTime.UtcNow;
        dos.Status = DosStatus.Active;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("SignDoS", $"{dos.Reference} — PFSO {pfsoName} / SHIP {shipName}");
        return true;
    }

    public string NextVesselReference(int year) => NextReference("ESC", year, _db.VesselCalls.IgnoreQueryFilters().Select(v => v.Reference));
    public string NextDosReference(int year) => NextReference("DOS", year, _db.DeclarationsOfSecurity.Select(d => d.Reference));

    private static string NextReference(string prefix, int year, IEnumerable<string> existingRefs)
    {
        var fullPrefix = $"{prefix}-{year}-";
        var last = existingRefs
            .Where(r => r.StartsWith(fullPrefix))
            .AsEnumerable()
            .Select(r => int.TryParse(r[fullPrefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return fullPrefix + (last + 1).ToString("D4");
    }
}
