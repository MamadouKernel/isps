using IspsDashboard.Models.ViewModels;

namespace IspsDashboard.Services.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsViewModel> Build12MonthAsync();
}
