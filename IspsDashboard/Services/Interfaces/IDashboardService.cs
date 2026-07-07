using IspsDashboard.Models.ViewModels;

namespace IspsDashboard.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> BuildDashboardAsync();
    Task<AdminViewModel> BuildAdminAsync();
    void InvalidateCache();
}
