using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IPdfReportService _pdf;
    private readonly IClock _clock;
    private readonly IAuditService _audit;

    public ReportsController(IPdfReportService pdf, IClock clock, IAuditService audit)
    {
        _pdf = pdf;
        _clock = clock;
        _audit = audit;
    }

    public IActionResult Index()
    {
        var now = _clock.Today;
        ViewBag.DefaultYear = now.Year;
        ViewBag.DefaultMonth = now.Month;
        ViewBag.Years = Enumerable.Range(now.Year - 2, 3).Reverse().ToList();
        return View();
    }

    [HttpGet("Reports/Monthly/{year:int}/{month:int}")]
    public async Task<IActionResult> Monthly(int year, int month)
    {
        if (year < 2020 || month < 1 || month > 12) return BadRequest();
        var bytes = await _pdf.GenerateMonthlyReportAsync(year, month);
        await _audit.LogAsync("ExportMonthlyReport", $"{year}-{month:D2} ({bytes.Length} octets)");
        return File(bytes, "application/pdf", $"Rapport-Surete-{year}-{month:D2}.pdf");
    }
}
