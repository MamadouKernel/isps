using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Services.Interfaces;

public interface INonConformityService
{
    Task<IReadOnlyList<NonConformity>> GetOpenAsync();
    Task<IReadOnlyList<NonConformity>> SearchAsync(NonConformityStatus? status);
    Task<NonConformity?> GetByIdAsync(int id);
    Task<NonConformity> CreateAsync(NonConformity input);
    Task<bool> UpdateAsync(NonConformity input);
    Task<bool> CloseAsync(int id, string evidence);
    string NextReference(int year);
}
