using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class AnalyticsController : Controller
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics) => _analytics = analytics;

    public async Task<IActionResult> Index()
        => View(await _analytics.Build12MonthAsync());
}
