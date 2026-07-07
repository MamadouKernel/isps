using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class VisitorsController : Controller
{
    private readonly IVisitorService _visitors;
    private readonly ISoftDeleteService _softDelete;

    public VisitorsController(IVisitorService visitors, ISoftDeleteService softDelete)
    {
        _visitors = visitors;
        _softDelete = softDelete;
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<Visitor>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Visiteur supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index(VisitorStatus? status, DateTime? from, DateTime? to)
    {
        ViewBag.OnSite = await _visitors.GetOnSiteAsync();
        ViewBag.Today = await _visitors.GetTodayAsync();
        ViewBag.Filtered = await _visitors.SearchAsync(status, from, to);
        ViewBag.Status = status;
        ViewBag.From = from;
        ViewBag.To = to;
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var v = await _visitors.GetByIdAsync(id);
        return v is null ? NotFound() : View(v);
    }

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create() => View(new Visitor { ScheduledArrival = DateTime.Now.AddHours(1) });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(Visitor input)
    {
        ModelState.Remove(nameof(Visitor.Reference)); // générée côté serveur
        if (!ModelState.IsValid) return View(input);
        var created = await _visitors.CreateAsync(input);
        TempData["Success"] = $"Visiteur pré-enregistré : {created.Reference}";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var v = await _visitors.GetByIdAsync(id);
        return v is null ? NotFound() : View(v);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, Visitor input)
    {
        if (id != input.Id) return BadRequest();
        ModelState.Remove(nameof(Visitor.Reference));
        if (!ModelState.IsValid) return View(input);
        try
        {
            var ok = await _visitors.UpdateAsync(input, input.RowVersion ?? Array.Empty<byte>());
            if (!ok) return NotFound();
            TempData["Success"] = "Visiteur mis à jour.";
            return RedirectToAction(nameof(Index));
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty, "Modifié entre-temps. Rechargez la page.");
            return View(input);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> CheckIn(int id, string badgeIssued)
    {
        var ok = await _visitors.CheckInAsync(id, User.Identity?.Name ?? "system", badgeIssued);
        TempData[ok ? "Success" : "Error"] = ok ? "Visiteur enregistré sur site." : "Action impossible.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> CheckOut(int id)
    {
        var ok = await _visitors.CheckOutAsync(id, User.Identity?.Name ?? "system");
        TempData[ok ? "Success" : "Error"] = ok ? "Sortie enregistrée." : "Action impossible.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Cancel(int id)
    {
        await _visitors.CancelAsync(id);
        TempData["Success"] = "Visite annulée.";
        return RedirectToAction(nameof(Index));
    }
}
