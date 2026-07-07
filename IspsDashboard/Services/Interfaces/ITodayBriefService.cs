using IspsDashboard.Models.ViewModels;

namespace IspsDashboard.Services.Interfaces;

public interface ITodayBriefService
{
    Task<TodayBriefViewModel> BuildAsync();
}
