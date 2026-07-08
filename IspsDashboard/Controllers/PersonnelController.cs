using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class PersonnelController : Controller
{
    private const long MaxPhotoBytes = 3_000_000;
    private static readonly string[] AllowedPhotoExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly IPersonnelService _personnel;
    private readonly IHabilitationService _habilitations;
    private readonly IWebHostEnvironment _env;
    private readonly ISoftDeleteService _softDelete;

    public PersonnelController(
        IPersonnelService personnel,
        IHabilitationService habilitations,
        IWebHostEnvironment env,
        ISoftDeleteService softDelete)
    {
        _personnel = personnel;
        _habilitations = habilitations;
        _env = env;
        _softDelete = softDelete;
    }

    public async Task<IActionResult> Index() => View(await _personnel.GetAllAsync());

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create() => View(new Agent());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(Agent input)
    {
        if (!ModelState.IsValid) return View(input);
        var agent = await _personnel.CreateAsync(input);
        TempData["Success"] = $"Agent {agent.Name} créé.";
        return RedirectToAction(nameof(Details), new { id = agent.Id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<Agent>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Agent retiré (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var agent = await _personnel.GetByIdAsync(id);
        if (agent is null) return NotFound();
        ViewBag.HabilitationService = _habilitations;
        return View(agent);
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var agent = await _personnel.GetByIdAsync(id);
        return agent is null ? NotFound() : View(agent);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> Edit(int id, Agent input, IFormFile? photo)
    {
        if (id != input.Id) return BadRequest();
        if (string.IsNullOrWhiteSpace(input.Email)) ModelState.Remove(nameof(Agent.Email)); // email optionnel
        if (!ModelState.IsValid) return View(input);

        if (photo is { Length: > 0 } && photo.Length <= MaxPhotoBytes)
        {
            var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
            if (AllowedPhotoExtensions.Contains(ext))
            {
                var dir = Path.Combine(_env.WebRootPath, "uploads", "personnel");
                Directory.CreateDirectory(dir);
                var safe = $"agent-{id}-{Guid.NewGuid():N}{ext}";
                await using var stream = System.IO.File.Create(Path.Combine(dir, safe));
                await photo.CopyToAsync(stream);
                input.PhotoPath = $"/uploads/personnel/{safe}";
            }
        }
        else
        {
            var existing = await _personnel.GetByIdAsync(id);
            input.PhotoPath = existing?.PhotoPath ?? string.Empty;
        }

        try
        {
            var ok = await _personnel.UpdateAsync(input, input.RowVersion ?? Array.Empty<byte>());
            if (!ok) return NotFound();
            TempData["Success"] = "Profil mis à jour.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty, "Modifié entre-temps par un autre utilisateur. Rechargez la page.");
            return View(input);
        }
    }
}
