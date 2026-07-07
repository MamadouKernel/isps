using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class ZonesController : Controller
{
    private readonly IRestrictedZoneService _zones;
    private readonly ISoftDeleteService _softDelete;

    public ZonesController(IRestrictedZoneService zones, ISoftDeleteService softDelete)
    {
        _zones = zones;
        _softDelete = softDelete;
    }

    public async Task<IActionResult> Index() => View(await _zones.GetAllAsync());

    public async Task<IActionResult> Details(int id)
    {
        var zone = await _zones.GetByIdAsync(id);
        return zone is null ? NotFound() : View(zone);
    }

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create() => View(new RestrictedZone());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(RestrictedZone input)
    {
        if (!ModelState.IsValid) return View(input);
        var z = await _zones.CreateAsync(input);
        TempData["Success"] = $"Zone {z.Code} créée.";
        return RedirectToAction(nameof(Details), new { id = z.Id });
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var zone = await _zones.GetByIdAsync(id);
        return zone is null ? NotFound() : View(zone);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, RestrictedZone input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return View(input);
        try
        {
            var ok = await _zones.UpdateAsync(input, input.RowVersion ?? Array.Empty<byte>());
            if (!ok) return NotFound();
            TempData["Success"] = "Zone mise à jour.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty, "Modifiée entre-temps par un autre utilisateur. Rechargez la page.");
            return View(input);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> SetStatus(int id, ZoneStatus status)
    {
        await _zones.SetStatusAsync(id, status);
        TempData["Success"] = $"État de la zone : {status}";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<RestrictedZone>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Zone supprimée (corbeille).";
        return RedirectToAction(nameof(Index));
    }
}
