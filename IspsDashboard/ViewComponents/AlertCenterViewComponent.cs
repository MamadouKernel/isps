using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.ViewComponents;

public class AlertCenterViewComponent : ViewComponent
{
    private readonly IAlertService _alerts;

    public AlertCenterViewComponent(IAlertService alerts) => _alerts = alerts;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _alerts.BuildAsync();
        return View(model);
    }
}
