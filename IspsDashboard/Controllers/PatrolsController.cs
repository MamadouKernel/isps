using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class PatrolsController : Controller
{
    private readonly IPatrolService _patrols;
    private readonly IBadgePdfService _badgePdf;
    private readonly IAuditService _audit;

    public PatrolsController(IPatrolService patrols, IBadgePdfService badgePdf, IAuditService audit)
    {
        _patrols = patrols;
        _badgePdf = badgePdf;
        _audit = audit;
    }

    /// <summary>Planche PDF des QR codes des checkpoints, à imprimer et coller sur le terrain.</summary>
    public async Task<IActionResult> QrSheet()
    {
        var checkpoints = await _patrols.GetActiveCheckpointsAsync();
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var pdf = _badgePdf.BuildCheckpointSheet(checkpoints, baseUrl);
        await _audit.LogAsync("ExportCheckpointQrSheet", $"{checkpoints.Count} checkpoints");
        return File(pdf, "application/pdf", "Checkpoints-QR.pdf");
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Checkpoints = await _patrols.GetActiveCheckpointsAsync();
        ViewBag.Recent = await _patrols.GetRecentScansAsync();
        ViewBag.LastByCheckpoint = await _patrols.GetLastScanByCheckpointAsync();
        ViewBag.Overdue = await _patrols.GetOverdueAsync();
        return View();
    }

    /// <summary>Page de scan accessible via QR : /Patrols/Scan/CP-01</summary>
    [HttpGet("Patrols/Scan/{code}")]
    public async Task<IActionResult> Scan(string code)
    {
        var checkpoint = await _patrols.GetByCodeAsync(code);
        if (checkpoint is null)
        {
            TempData["Error"] = $"Checkpoint inconnu : {code}";
            return RedirectToAction(nameof(Index));
        }
        return View(checkpoint);
    }

    [HttpPost("Patrols/Scan/{code}"), ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Scan(
        string code,
        string? observations,
        string? anomalyType,
        double? latitude,
        double? longitude)
    {
        try
        {
            await _patrols.RecordScanAsync(
                code,
                User.Identity?.Name ?? "anonyme",
                null,
                observations,
                latitude,
                longitude,
                anomalyType);
            TempData["Success"] = $"Passage enregistré au checkpoint {code}.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
