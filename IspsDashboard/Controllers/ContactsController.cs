using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class ContactsController : Controller
{
    private readonly IContactService _contacts;
    private readonly ISoftDeleteService _softDelete;

    public ContactsController(IContactService contacts, ISoftDeleteService softDelete)
    {
        _contacts = contacts;
        _softDelete = softDelete;
    }

    public async Task<IActionResult> Index(ContactType? type)
    {
        ViewBag.Contacts = await _contacts.GetAllAsync(type);
        ViewBag.Recent = await _contacts.GetRecentInteractionsAsync();
        ViewBag.Type = type;
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var c = await _contacts.GetByIdAsync(id);
        return c is null ? NotFound() : View(c);
    }

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create() => View(new ExternalContact());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(ExternalContact input)
    {
        if (string.IsNullOrWhiteSpace(input.Email)) ModelState.Remove(nameof(ExternalContact.Email)); // email optionnel
        if (!ModelState.IsValid) return View(input);
        var c = await _contacts.CreateAsync(input);
        TempData["Success"] = $"Contact {c.Name} ajouté.";
        return RedirectToAction(nameof(Details), new { id = c.Id });
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var c = await _contacts.GetByIdAsync(id);
        return c is null ? NotFound() : View(c);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, ExternalContact input)
    {
        if (id != input.Id) return BadRequest();
        if (string.IsNullOrWhiteSpace(input.Email)) ModelState.Remove(nameof(ExternalContact.Email));
        if (!ModelState.IsValid) return View(input);
        var ok = await _contacts.UpdateAsync(input);
        if (!ok) return NotFound();
        TempData["Success"] = "Contact mis à jour.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> LogInteraction(int id, [FromForm] ContactInteraction entry)
    {
        entry.HandledBy = string.IsNullOrEmpty(entry.HandledBy) ? (User.Identity?.Name ?? "system") : entry.HandledBy;
        await _contacts.LogInteractionAsync(id, entry);
        TempData["Success"] = "Interaction enregistrée.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<ExternalContact>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Contact supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }
}
