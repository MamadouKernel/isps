using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class NonConformitiesController : Controller
{
    private readonly INonConformityService _service;
    private readonly ISoftDeleteService _softDelete;

    public NonConformitiesController(INonConformityService service, ISoftDeleteService softDelete)
    {
        _service = service;
        _softDelete = softDelete;
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<NonConformity>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Non-conformité supprimée (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index(NonConformityStatus? status)
    {
        var list = await _service.SearchAsync(status);
        ViewBag.Status = status;
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var nc = await _service.GetByIdAsync(id);
        return nc is null ? NotFound() : View(nc);
    }

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create() => View(new NonConformity { IdentifiedAt = DateTime.Now });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(NonConformity input)
    {
        ModelState.Remove(nameof(NonConformity.Reference)); // générée côté serveur
        if (!ModelState.IsValid) return View(input);
        var created = await _service.CreateAsync(input);
        TempData["Success"] = $"Non-conformité enregistrée : {created.Reference}";
        return RedirectToAction(nameof(Details), new { id = created.Id });
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var nc = await _service.GetByIdAsync(id);
        return nc is null ? NotFound() : View(nc);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, NonConformity input)
    {
        if (id != input.Id) return BadRequest();
        ModelState.Remove(nameof(NonConformity.Reference));
        if (!ModelState.IsValid) return View(input);
        try
        {
            var ok = await _service.UpdateAsync(input, input.RowVersion ?? Array.Empty<byte>());
            if (!ok) return NotFound();
            TempData["Success"] = "Non-conformité mise à jour.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty, "Modifié entre-temps par un autre utilisateur. Rechargez la page.");
            return View(input);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Close(int id, string evidence)
    {
        var ok = await _service.CloseAsync(id, evidence);
        TempData[ok ? "Success" : "Error"] = ok ? "Non-conformité levée." : "Introuvable.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
