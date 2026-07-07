using IspsDashboard.Models.ViewModels;

namespace IspsDashboard.Services.Interfaces;

public interface IAlertService
{
    Task<AlertCenterViewModel> BuildAsync();
}
