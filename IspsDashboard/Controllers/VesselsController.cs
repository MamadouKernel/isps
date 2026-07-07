using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Controllers;

[Authorize]
public class VesselsController : Controller
{
    private readonly IVesselService _vessels;
    private readonly IMarsecService _marsec;
    private readonly ISoftDeleteService _softDelete;

    public VesselsController(IVesselService vessels, IMarsecService marsec, ISoftDeleteService softDelete)
    {
        _vessels = vessels;
        _marsec = marsec;
        _softDelete = softDelete;
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<VesselCall>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Escale supprimée (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index(VesselCallStatus? status, DateTime? from, DateTime? to)
    {
        ViewBag.Upcoming = await _vessels.GetUpcomingAsync(14);
        ViewBag.Filtered = await _vessels.SearchAsync(status, from, to);
        ViewBag.Status = status;
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var vessel = await _vessels.GetByIdAsync(id);
        if (vessel is null) return NotFound();
        ViewBag.PortLevel = await _marsec.GetCurrentLevelAsync();
        return View(vessel);
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create()
    {
        ViewBag.PortLevel = await _marsec.GetCurrentLevelAsync();
        return View(new VesselCall { Eta = DateTime.Today.AddHours(8), ShipIspsLevel = 1 });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(VesselCall input)
    {
        ModelState.Remove(nameof(VesselCall.Reference)); // générée côté serveur
        if (!ModelState.IsValid)
        {
            ViewBag.PortLevel = await _marsec.GetCurrentLevelAsync();
            return View(input);
        }
        var created = await _vessels.CreateAsync(input);
        TempData["Success"] = $"Escale enregistrée : {created.Reference}";
        return RedirectToAction(nameof(Details), new { id = created.Id });
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var vessel = await _vessels.GetByIdAsync(id);
        if (vessel is null) return NotFound();
        ViewBag.PortLevel = await _marsec.GetCurrentLevelAsync();
        return View(vessel);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, VesselCall input)
    {
        ModelState.Remove(nameof(VesselCall.Reference));
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.PortLevel = await _marsec.GetCurrentLevelAsync();
            return View(input);
        }
        try
        {
            var ok = await _vessels.UpdateAsync(input, input.RowVersion ?? Array.Empty<byte>());
            if (!ok) return NotFound();
            TempData["Success"] = "Escale mise à jour.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty, "Modifiée entre-temps. Rechargez la page.");
            ViewBag.PortLevel = await _marsec.GetCurrentLevelAsync();
            return View(input);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> ChangeStatus(int id, VesselCallStatus newStatus)
    {
        await _vessels.ChangeStatusAsync(id, newStatus);
        TempData["Success"] = $"Statut mis à jour : {newStatus}";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> CreateDos(int id, [FromForm] DeclarationOfSecurity input)
    {
        ModelState.Remove(nameof(DeclarationOfSecurity.Reference)); // générée côté serveur
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Données DoS invalides.";
            return RedirectToAction(nameof(Details), new { id });
        }
        var dos = await _vessels.CreateDosAsync(id, input);
        TempData["Success"] = $"DoS créée : {dos.Reference}";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> SignDos(int id, int dosId, string pfsoName, string shipName)
    {
        var ok = await _vessels.SignDosAsync(dosId, pfsoName, shipName);
        TempData[ok ? "Success" : "Error"] = ok ? "DoS signée et activée." : "DoS introuvable.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
