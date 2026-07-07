using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IspsDashboard.Models.Entities;

namespace IspsDashboard.Controllers;

[Authorize]
public class MarsecController : Controller
{
    private readonly IMarsecService _marsec;
    private readonly UserManager<ApplicationUser> _userManager;

    public MarsecController(IMarsecService marsec, UserManager<ApplicationUser> userManager)
    {
        _marsec = marsec;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.CurrentLevel = await _marsec.GetCurrentLevelAsync();
        ViewBag.History = await _marsec.GetHistoryAsync();
        ViewBag.Template1 = _marsec.GetChecklistTemplate(1);
        ViewBag.Template2 = _marsec.GetChecklistTemplate(2);
        ViewBag.Template3 = _marsec.GetChecklistTemplate(3);
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> RequestChange(int newLevel, string reason, string decisionSource, string decidedBy)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            TempData["Error"] = "Le motif est obligatoire pour tracer la décision.";
            return RedirectToAction(nameof(Index));
        }
        try
        {
            var change = await _marsec.RequestChangeAsync(newLevel, reason, decisionSource, decidedBy,
                _userManager.GetUserId(User));
            TempData["Success"] = $"Niveau MARSEC modifié : {change.FromLevel} → {change.ToLevel}.";
        }
        catch (ArgumentOutOfRangeException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> CompleteItem(int itemId, string? notes)
    {
        await _marsec.CompleteChecklistItemAsync(itemId, User.Identity?.Name ?? "system", notes);
        return RedirectToAction(nameof(Index));
    }
}
