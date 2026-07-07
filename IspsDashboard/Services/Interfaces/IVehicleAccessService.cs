using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Services.Interfaces;

public interface IVehicleAccessService
{
    Task<IReadOnlyList<VehicleAccess>> SearchAsync(AccessDirection? direction, DateTime? from, DateTime? to, string? query);
    Task<IReadOnlyList<VehicleAccess>> GetTodayAsync();
    Task<int> CountTodayAsync();
    Task<VehicleAccess> CreateAsync(VehicleAccess input);
    Task<VehicleAccess?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(VehicleAccess input);
    string NextReference(int year);
}
