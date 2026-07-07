using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Services.Interfaces;

public interface ICameraService
{
    Task<IReadOnlyList<Camera>> GetAllAsync();
    Task<Camera?> GetByIdAsync(int id);
    Task<Camera> CreateAsync(Camera input);
    Task<bool> UpdateAsync(Camera input);
    Task<bool> ChangeStatusAsync(int id, CameraStatus newStatus, string by, string? notes);
    Task<CameraMaintenance> LogMaintenanceAsync(int cameraId, CameraMaintenance entry);
    Task<int> CountAvailableAsync();
    Task<double> AvailabilityPercentAsync();
}
