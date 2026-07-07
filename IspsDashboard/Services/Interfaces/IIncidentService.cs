using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;

namespace IspsDashboard.Services.Interfaces;

public interface IIncidentService
{
    Task<IReadOnlyList<Incident>> SearchAsync(IncidentSearchCriteria criteria);
    Task<Incident?> GetByIdAsync(int id);
    Task<Incident> CreateAsync(IncidentEditViewModel input, string createdById);
    Task<bool> UpdateAsync(int id, IncidentEditViewModel input, byte[] rowVersion);
    Task<bool> ChangeStatusAsync(int id, IncidentStatus newStatus, string? userId);
    Task<int> CountByStatusAsync(IncidentStatus status);
    Task<IReadOnlyList<Incident>> GetRecentAsync(int take = 5);
    string NextReference(int yearReference);
}
