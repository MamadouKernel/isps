using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Services.Interfaces;

public interface IVisitorService
{
    Task<IReadOnlyList<Visitor>> GetTodayAsync();
    Task<IReadOnlyList<Visitor>> GetOnSiteAsync();
    Task<int> CountOnSiteAsync();
    Task<IReadOnlyList<Visitor>> SearchAsync(VisitorStatus? status, DateTime? from, DateTime? to);
    Task<Visitor?> GetByIdAsync(int id);
    Task<Visitor> CreateAsync(Visitor input);
    Task<bool> UpdateAsync(Visitor input, byte[] rowVersion);
    Task<bool> CheckInAsync(int id, string by, string badgeIssued);
    Task<bool> CheckOutAsync(int id, string by);
    Task<bool> CancelAsync(int id);
    string NextReference(int year);
}
