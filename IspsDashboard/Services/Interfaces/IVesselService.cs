using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Services.Interfaces;

public interface IVesselService
{
    Task<IReadOnlyList<VesselCall>> GetUpcomingAsync(int daysAhead = 30);
    Task<IReadOnlyList<VesselCall>> SearchAsync(VesselCallStatus? status, DateTime? from, DateTime? to);
    Task<VesselCall?> GetByIdAsync(int id);
    Task<VesselCall> CreateAsync(VesselCall input);
    Task<bool> UpdateAsync(VesselCall input, byte[] rowVersion);
    Task<bool> ChangeStatusAsync(int id, VesselCallStatus newStatus);
    Task<DeclarationOfSecurity> CreateDosAsync(int vesselCallId, DeclarationOfSecurity input);
    Task<bool> SignDosAsync(int dosId, string pfsoName, string shipName);
    string NextVesselReference(int year);
    string NextDosReference(int year);
}
