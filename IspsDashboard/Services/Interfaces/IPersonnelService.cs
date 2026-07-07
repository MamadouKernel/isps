using IspsDashboard.Models.Entities;

namespace IspsDashboard.Services.Interfaces;

public interface IPersonnelService
{
    Task<IReadOnlyList<Agent>> GetAllAsync();
    Task<Agent?> GetByIdAsync(int id);
    Task<Agent> CreateAsync(Agent input);
    Task<bool> UpdateAsync(Agent input);
    Task<int> CountPresentAsync();
}
