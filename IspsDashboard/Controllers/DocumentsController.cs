using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class DocumentsController : Controller
{
    private const long MaxBytes = 20_000_000; // 20 Mo
    private static readonly string[] Allowed = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg" };

    private readonly IDocumentService _docs;
    private readonly IWebHostEnvironment _env;
    private readonly ISoftDeleteService _softDelete;

    public DocumentsController(IDocumentService docs, IWebHostEnvironment env, ISoftDeleteService softDelete)
    {
        _docs = docs;
        _env = env;
        _softDelete = softDelete;
    }

    public async Task<IActionResult> Index(DocumentCategory? category)
    {
        ViewBag.Documents = await _docs.GetAllAsync(category);
        ViewBag.DueForReview = await _docs.GetDueForReviewAsync(30);
        ViewBag.Category = category;
        return View();
    }

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create() => View(new SecurityDocument());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    [RequestSizeLimit(25_000_000)]
    public async Task<IActionResult> Create(SecurityDocument input, IFormFile? file)
    {
        if (!ModelState.IsValid) return View(input);

        if (file is { Length: > 0 })
        {
            if (file.Length > MaxBytes)
            {
                ModelState.AddModelError(string.Empty, "Fichier trop volumineux (max 20 Mo).");
                return View(input);
            }
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!Allowed.Contains(ext))
            {
                ModelState.AddModelError(string.Empty, "Format non autorisé.");
                return View(input);
            }
            var dir = Path.Combine(_env.WebRootPath, "uploads", "documents");
            Directory.CreateDirectory(dir);
            var safe = $"{Guid.NewGuid():N}{ext}";
            await using (var stream = System.IO.File.Create(Path.Combine(dir, safe)))
                await file.CopyToAsync(stream);
            input.FilePath = $"/uploads/documents/{safe}";
            input.FileName = Path.GetFileName(file.FileName);
            input.FileSizeBytes = file.Length;
        }

        var created = await _docs.CreateAsync(input);
        TempData["Success"] = $"Document « {created.Title} » v{created.Version} enregistré.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var doc = await _docs.GetByIdAsync(id);
        return doc is null ? NotFound() : View(doc);
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var doc = await _docs.GetByIdAsync(id);
        return doc is null ? NotFound() : View(doc);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    [RequestSizeLimit(25_000_000)]
    public async Task<IActionResult> Edit(int id, SecurityDocument input, IFormFile? file)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return View(input);

        if (file is { Length: > 0 })
        {
            if (file.Length > MaxBytes)
            {
                ModelState.AddModelError(string.Empty, "Fichier trop volumineux (max 20 Mo).");
                return View(input);
            }
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!Allowed.Contains(ext))
            {
                ModelState.AddModelError(string.Empty, "Format non autorisé.");
                return View(input);
            }
            var dir = Path.Combine(_env.WebRootPath, "uploads", "documents");
            Directory.CreateDirectory(dir);
            var safe = $"{Guid.NewGuid():N}{ext}";
            await using (var stream = System.IO.File.Create(Path.Combine(dir, safe)))
                await file.CopyToAsync(stream);
            input.FilePath = $"/uploads/documents/{safe}";
            input.FileName = Path.GetFileName(file.FileName);
            input.FileSizeBytes = file.Length;
        }

        var ok = await _docs.UpdateAsync(input);
        if (!ok) return NotFound();
        TempData["Success"] = "Document mis à jour.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> ChangeStatus(int id, DocumentStatus status)
    {
        await _docs.UpdateStatusAsync(id, status);
        TempData["Success"] = $"Statut mis à jour : {status}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<SecurityDocument>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Document supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }
}
