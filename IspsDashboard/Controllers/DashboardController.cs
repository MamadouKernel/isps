using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ITodayBriefService _todayBrief;

    public DashboardController(IDashboardService dashboardService, ITodayBriefService todayBrief)
    {
        _dashboardService = dashboardService;
        _todayBrief = todayBrief;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _dashboardService.BuildDashboardAsync();
        ViewBag.TodayBrief = await _todayBrief.BuildAsync();
        return View(model);
    }
}
