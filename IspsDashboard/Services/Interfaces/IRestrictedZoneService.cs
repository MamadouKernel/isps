using IspsDashboard.Models.Entities;

namespace IspsDashboard.Services.Interfaces;

public interface IRestrictedZoneService
{
    Task<IReadOnlyList<RestrictedZone>> GetAllAsync();
    Task<RestrictedZone?> GetByIdAsync(int id);
    Task<RestrictedZone> CreateAsync(RestrictedZone input);
    Task<bool> UpdateAsync(RestrictedZone input, byte[] rowVersion);
    Task<bool> SetStatusAsync(int id, Models.Enums.ZoneStatus status);
    Task<bool> DeleteAsync(int id);
}
