using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Controllers;

[Authorize]
public class AccessPassController : Controller
{
    private readonly IAccessPassService _passes;
    private readonly IBadgePdfService _badgePdf;
    private readonly IExportService _export;
    private readonly Data.ApplicationDbContext _db;
    private readonly ISoftDeleteService _softDelete;

    public AccessPassController(IAccessPassService passes, IBadgePdfService badgePdf, IExportService export, Data.ApplicationDbContext db, ISoftDeleteService softDelete)
    {
        _passes = passes;
        _badgePdf = badgePdf;
        _export = export;
        _db = db;
        _softDelete = softDelete;
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<AccessPass>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Laissez-passer supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    private async Task<TableData> BuildExportAsync(AccessPassType? type, AccessPassCategory? category, string? query)
    {
        var passes = await _passes.SearchAsync(type, category, query);
        var headers = new[] { "Référence", "Type", "Catégorie", "Nom", "Prénom", "Contact", "Matricule", "Email", "Immatriculation", "Société", "Édition", "Fin", "Statut" };
        var rows = passes.Select(p => (IReadOnlyList<string>)new[]
        {
            p.Reference, p.Type.ToString(), p.Category.ToString(), p.LastName, p.FirstName, p.Contact,
            p.Matricule, p.Email, p.Plate, p.Company,
            p.IssueDate.ToString("dd/MM/yyyy"), p.EndDate.ToString("dd/MM/yyyy"), _passes.GetState(p).ToString()
        }).ToList();
        return new TableData("Laissez-passer d'accès", headers, rows);
    }

    public async Task<IActionResult> ExportExcel(AccessPassType? type, AccessPassCategory? category, string? query)
        => File(_export.ToExcel(await BuildExportAsync(type, category, query)),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Laissez-passer-{DateTime.Now:yyyyMMdd}.xlsx");

    public async Task<IActionResult> ExportPdf(AccessPassType? type, AccessPassCategory? category, string? query)
        => File(_export.ToPdf(await BuildExportAsync(type, category, query)), "application/pdf", $"Laissez-passer-{DateTime.Now:yyyyMMdd}.pdf");

    /// <summary>Badge PDF imprimable d'un laissez-passer (avec QR de vérification).</summary>
    public async Task<IActionResult> Badge(int id)
    {
        var pass = await _passes.GetByIdAsync(id);
        if (pass is null) return NotFound();
        var terminal = (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
            .FirstOrDefaultAsync(_db.DashboardSettings.AsQueryable()))?.TerminalTitle ?? "Côte d'Ivoire Terminal";
        var pdf = _badgePdf.BuildAccessPassBadge(pass, terminal);
        return File(pdf, "application/pdf", $"Badge-{pass.Reference}.pdf");
    }

    public async Task<IActionResult> Index(AccessPassType? type, AccessPassCategory? category, string? query)
    {
        ViewBag.Passes = await _passes.SearchAsync(type, category, query);
        ViewBag.Service = _passes;
        ViewBag.Type = type;
        ViewBag.Category = category;
        ViewBag.Query = query;
        ViewBag.CountActive = await _passes.CountActiveAsync();
        ViewBag.CountExpiring = await _passes.CountExpiringAsync(7);
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var pass = await _passes.GetByIdAsync(id);
        if (pass is null) return NotFound();
        ViewBag.Service = _passes;
        return View(pass);
    }

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create(AccessPassType type = AccessPassType.Journalier, AccessPassCategory category = AccessPassCategory.Personne)
    {
        var issue = DateTime.Today;
        return View(new AccessPass
        {
            Type = type,
            Category = category,
            IssueDate = issue,
            EndDate = _passes.ComputeDefaultEndDate(type, issue)
        });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(AccessPass input)
    {
        ModelState.Remove(nameof(AccessPass.Reference)); // générée côté serveur
        if (string.IsNullOrWhiteSpace(input.Email)) ModelState.Remove(nameof(AccessPass.Email)); // email optionnel
        if (input.EndDate == default)
            input.EndDate = _passes.ComputeDefaultEndDate(input.Type, input.IssueDate);
        if (!ModelState.IsValid) return View(input);

        if (string.IsNullOrEmpty(input.IssuedBy)) input.IssuedBy = User.Identity?.Name ?? "system";
        var created = await _passes.CreateAsync(input);
        TempData["Success"] = $"Laissez-passer délivré : {created.Reference}";
        return RedirectToAction(nameof(Index), new { type = input.Type });
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var pass = await _passes.GetByIdAsync(id);
        return pass is null ? NotFound() : View(pass);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, AccessPass input)
    {
        ModelState.Remove(nameof(AccessPass.Reference));
        if (string.IsNullOrWhiteSpace(input.Email)) ModelState.Remove(nameof(AccessPass.Email));
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return View(input);
        try
        {
            var ok = await _passes.UpdateAsync(input, input.RowVersion ?? Array.Empty<byte>());
            if (!ok) return NotFound();
            TempData["Success"] = "Laissez-passer mis à jour.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty, "Modifié entre-temps par un autre utilisateur. Rechargez la page.");
            return View(input);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Revoke(int id)
    {
        await _passes.RevokeAsync(id);
        TempData["Success"] = "Laissez-passer révoqué.";
        return RedirectToAction(nameof(Index));
    }
}
