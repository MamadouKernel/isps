using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IspsDashboard.Data;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Controllers;

[Authorize]
public class IncidentsController : Controller
{
    private const long MaxAttachmentBytes = 5_000_000; // 5 Mo
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };

    private readonly IIncidentService _incidents;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;
    private readonly IExportService _export;
    private readonly ISoftDeleteService _softDelete;

    public IncidentsController(
        IIncidentService incidents,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env,
        ApplicationDbContext db,
        IAuditService audit,
        IExportService export,
        ISoftDeleteService softDelete)
    {
        _incidents = incidents;
        _userManager = userManager;
        _env = env;
        _db = db;
        _audit = audit;
        _export = export;
        _softDelete = softDelete;
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<Incident>(id, _userManager.GetUserId(User));
        TempData["Success"] = "Incident supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index([FromQuery] IncidentSearchCriteria criteria)
    {
        criteria ??= new IncidentSearchCriteria();
        var list = await _incidents.SearchAsync(criteria);
        var model = new IncidentListViewModel
        {
            Incidents = list,
            Criteria = criteria,
            TotalOpen = await _incidents.CountByStatusAsync(IncidentStatus.Ouvert),
            TotalInvestigation = await _incidents.CountByStatusAsync(IncidentStatus.EnInvestigation),
            TotalClosed = await _incidents.CountByStatusAsync(IncidentStatus.Clos)
        };
        return View(model);
    }

    private async Task<TableData> BuildExportAsync(IncidentSearchCriteria criteria)
    {
        var list = await _incidents.SearchAsync(criteria);
        var headers = new[] { "Référence", "Survenu le", "Titre", "Catégorie", "Sévérité", "Statut", "Zone", "Déclaré par", "Enquêteur" };
        var rows = list.Select(i => (IReadOnlyList<string>)new[]
        {
            i.Reference, i.OccurredAt.ToString("dd/MM/yyyy HH:mm"), i.Title, i.Category.ToString(),
            i.Severity.ToString(), i.Status.ToString(), i.Zone, i.ReportedBy, i.Investigator
        }).ToList();
        return new TableData("Registre des incidents de sûreté", headers, rows);
    }

    public async Task<IActionResult> ExportExcel([FromQuery] IncidentSearchCriteria criteria)
        => File(_export.ToExcel(await BuildExportAsync(criteria ?? new IncidentSearchCriteria())),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Incidents-{DateTime.Now:yyyyMMdd}.xlsx");

    public async Task<IActionResult> ExportPdf([FromQuery] IncidentSearchCriteria criteria)
        => File(_export.ToPdf(await BuildExportAsync(criteria ?? new IncidentSearchCriteria())),
            "application/pdf", $"Incidents-{DateTime.Now:yyyyMMdd}.pdf");

    public async Task<IActionResult> Details(int id)
    {
        var entity = await _incidents.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return View(entity);
    }

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create() => View(new IncidentEditViewModel { OccurredAt = DateTime.Now });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    [RequestSizeLimit(30_000_000)]
    public async Task<IActionResult> Create(IncidentEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = _userManager.GetUserId(User) ?? string.Empty;
        var created = await _incidents.CreateAsync(model, userId);
        await SaveAttachmentsAsync(created.Id, model.NewAttachments);

        TempData["Success"] = $"Incident {created.Reference} déclaré.";
        return RedirectToAction(nameof(Details), new { id = created.Id });
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _incidents.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return View(EntityToViewModel(entity));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    [RequestSizeLimit(30_000_000)]
    public async Task<IActionResult> Edit(int id, IncidentEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var ok = await _incidents.UpdateAsync(id, model, model.RowVersion ?? Array.Empty<byte>());
            if (!ok) return NotFound();
            await SaveAttachmentsAsync(id, model.NewAttachments);
            TempData["Success"] = "Incident mis à jour.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty,
                "Cet incident a été modifié entre-temps par un autre utilisateur. Rechargez la page.");
            return View(model);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> ChangeStatus(int id, IncidentStatus newStatus)
    {
        var ok = await _incidents.ChangeStatusAsync(id, newStatus, _userManager.GetUserId(User));
        TempData[ok ? "Success" : "Error"] = ok
            ? $"Statut mis à jour : {newStatus}."
            : "Incident introuvable.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task SaveAttachmentsAsync(int incidentId, IList<IFormFile>? files)
    {
        if (files is null || files.Count == 0) return;

        var dir = Path.Combine(_env.WebRootPath, "uploads", "incidents", incidentId.ToString());
        Directory.CreateDirectory(dir);

        foreach (var file in files)
        {
            if (file.Length == 0 || file.Length > MaxAttachmentBytes) continue;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext)) continue;

            var safeName = $"{Guid.NewGuid():N}{ext}";
            var path = Path.Combine(dir, safeName);
            await using (var stream = System.IO.File.Create(path))
                await file.CopyToAsync(stream);

            _db.IncidentAttachments.Add(new IncidentAttachment
            {
                IncidentId = incidentId,
                FileName = Path.GetFileName(file.FileName),
                StoredPath = $"/uploads/incidents/{incidentId}/{safeName}",
                ContentType = file.ContentType,
                SizeBytes = file.Length
            });
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AddIncidentAttachment", $"Incident #{incidentId}: {files.Count} fichier(s)");
    }

    private static IncidentEditViewModel EntityToViewModel(Incident entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Category = entity.Category,
        Severity = entity.Severity,
        Status = entity.Status,
        OccurredAt = entity.OccurredAt,
        Zone = entity.Zone,
        ReportedBy = entity.ReportedBy,
        Investigator = entity.Investigator,
        Description = entity.Description,
        ActionsTaken = entity.ActionsTaken,
        LessonsLearned = entity.LessonsLearned,
        RowVersion = entity.RowVersion
    };
}
