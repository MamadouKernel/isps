using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class BriefingsController : Controller
{
    private readonly IBriefingService _briefings;
    private readonly IMarsecService _marsec;
    private readonly IClock _clock;
    private readonly ISoftDeleteService _softDelete;

    public BriefingsController(IBriefingService briefings, IMarsecService marsec, IClock clock, ISoftDeleteService softDelete)
    {
        _briefings = briefings;
        _marsec = marsec;
        _clock = clock;
        _softDelete = softDelete;
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<ShiftBriefing>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Briefing supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Latest = await _briefings.GetLatestAsync();
        ViewBag.Recent = await _briefings.GetRecentAsync();
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var b = await _briefings.GetByIdAsync(id);
        return b is null ? NotFound() : View(b);
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create()
    {
        var now = _clock.Now;
        var model = new ShiftBriefing
        {
            ShiftDate = now.Date,
            Slot = _briefings.DetermineCurrentSlot(now),
            CurrentMarsecLevel = await _marsec.GetCurrentLevelAsync()
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(ShiftBriefing input)
    {
        if (!ModelState.IsValid) return View(input);
        await _briefings.CreateAsync(input);
        TempData["Success"] = "Briefing enregistré.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var b = await _briefings.GetByIdAsync(id);
        return b is null ? NotFound() : View(b);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, ShiftBriefing input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return View(input);
        try
        {
            var ok = await _briefings.UpdateAsync(input, input.RowVersion ?? Array.Empty<byte>());
            if (!ok) return NotFound();
            TempData["Success"] = "Briefing mis à jour.";
            return RedirectToAction(nameof(Index));
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty, "Modifié entre-temps. Rechargez la page.");
            return View(input);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Acknowledge(int id)
    {
        await _briefings.AcknowledgeAsync(id, User.Identity?.Name ?? "system");
        TempData["Success"] = "Briefing pris en compte.";
        return RedirectToAction(nameof(Index));
    }
}
