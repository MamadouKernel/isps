using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class HabilitationsController : Controller
{
    private readonly IHabilitationService _service;
    private readonly ISoftDeleteService _softDelete;

    public HabilitationsController(IHabilitationService service, ISoftDeleteService softDelete)
    {
        _service = service;
        _softDelete = softDelete;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ExpiringSoon = await _service.GetExpiringSoonAsync();
        ViewBag.Expired = await _service.GetExpiredAsync();
        ViewBag.All = await _service.GetAllAsync();
        ViewBag.Service = _service;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(int agentId, [FromForm] Habilitation input)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Saisie invalide.";
            return RedirectToAction("Details", "Personnel", new { id = agentId });
        }
        await _service.CreateAsync(agentId, input);
        TempData["Success"] = "Habilitation ajoutée.";
        return RedirectToAction("Details", "Personnel", new { id = agentId });
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var h = await _service.GetByIdAsync(id);
        return h is null ? NotFound() : View(h);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, Habilitation input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return View(input);
        var agentId = await _service.UpdateAsync(input);
        if (agentId == 0) return NotFound();
        TempData["Success"] = "Habilitation mise à jour.";
        return RedirectToAction("Details", "Personnel", new { id = agentId });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id, int agentId)
    {
        await _softDelete.DeleteAsync<Habilitation>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Habilitation supprimée (corbeille).";
        return RedirectToAction("Details", "Personnel", new { id = agentId });
    }
}
