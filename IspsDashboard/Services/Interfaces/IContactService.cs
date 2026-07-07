using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Services.Interfaces;

public interface IContactService
{
    Task<IReadOnlyList<ExternalContact>> GetAllAsync(ContactType? type = null);
    Task<ExternalContact?> GetByIdAsync(int id);
    Task<ExternalContact> CreateAsync(ExternalContact input);
    Task<bool> UpdateAsync(ExternalContact input);
    Task<bool> DeleteAsync(int id);
    Task<ContactInteraction> LogInteractionAsync(int contactId, ContactInteraction entry);
    Task<IReadOnlyList<ContactInteraction>> GetRecentInteractionsAsync(int take = 20);
}
