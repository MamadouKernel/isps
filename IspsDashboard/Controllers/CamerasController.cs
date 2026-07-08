using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class CamerasController : Controller
{
    private readonly ICameraService _cameras;
    private readonly ISoftDeleteService _softDelete;

    public CamerasController(ICameraService cameras, ISoftDeleteService softDelete)
    {
        _cameras = cameras;
        _softDelete = softDelete;
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<Camera>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Caméra supprimée (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Cameras = await _cameras.GetAllAsync();
        ViewBag.Availability = await _cameras.AvailabilityPercentAsync();
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var cam = await _cameras.GetByIdAsync(id);
        return cam is null ? NotFound() : View(cam);
    }

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create() => View(new Camera());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(Camera input)
    {
        if (!ModelState.IsValid) return View(input);
        var c = await _cameras.CreateAsync(input);
        TempData["Success"] = $"Caméra {c.Code} créée.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var c = await _cameras.GetByIdAsync(id);
        return c is null ? NotFound() : View(c);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, Camera input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return View(input);
        try
        {
            var ok = await _cameras.UpdateAsync(input, input.RowVersion ?? Array.Empty<byte>());
            if (!ok) return NotFound();
            TempData["Success"] = "Caméra mise à jour.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty, "Modifié entre-temps par un autre utilisateur. Rechargez la page.");
            return View(input);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> ChangeStatus(int id, CameraStatus newStatus, string? notes)
    {
        await _cameras.ChangeStatusAsync(id, newStatus, User.Identity?.Name ?? "system", notes);
        TempData["Success"] = $"Statut mis à jour : {newStatus}";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Maintenance(int id, [FromForm] CameraMaintenance entry)
    {
        entry.PerformedBy = User.Identity?.Name ?? "system";
        await _cameras.LogMaintenanceAsync(id, entry);
        TempData["Success"] = "Intervention enregistrée.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
