using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class AuditsController : Controller
{
    private readonly IAuditCampaignService _audits;
    private readonly INonConformityService _nonConformities;
    private readonly ISoftDeleteService _softDelete;

    public AuditsController(IAuditCampaignService audits, INonConformityService nonConformities, ISoftDeleteService softDelete)
    {
        _audits = audits;
        _nonConformities = nonConformities;
        _softDelete = softDelete;
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<SecurityAudit>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Audit supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index() => View(await _audits.GetAllAsync());

    /// <summary>Génère une non-conformité à partir d'un constat d'audit non conforme.</summary>
    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> GenerateNonConformity(int auditId, int findingId)
    {
        var audit = await _audits.GetByIdAsync(auditId);
        var finding = audit?.Findings.FirstOrDefault(f => f.Id == findingId);
        if (audit is null || finding is null) return NotFound();

        var nc = await _nonConformities.CreateAsync(new NonConformity
        {
            Title = $"[{audit.Reference}] {finding.CheckItem}",
            Source = audit.Type == AuditType.Externe ? NonConformitySource.AuditExterne : NonConformitySource.AuditInterne,
            Description = string.IsNullOrWhiteSpace(finding.Observation)
                ? $"Constat non conforme lors de l'audit {audit.Reference} : {finding.CheckItem}"
                : finding.Observation,
            IdentifiedAt = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Owner = audit.Auditor
        });
        TempData["Success"] = $"Non-conformité {nc.Reference} créée depuis le constat d'audit.";
        return RedirectToAction(nameof(Details), new { id = auditId });
    }

    public async Task<IActionResult> Details(int id)
    {
        var audit = await _audits.GetByIdAsync(id);
        return audit is null ? NotFound() : View(audit);
    }

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create() => View(new SecurityAudit { ScheduledDate = DateTime.Today });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(SecurityAudit input, bool seedChecklist = true)
    {
        ModelState.Remove(nameof(SecurityAudit.Reference)); // générée côté serveur
        if (!ModelState.IsValid) return View(input);
        var created = await _audits.CreateAsync(input, seedChecklist);
        TempData["Success"] = $"Audit créé : {created.Reference}";
        return RedirectToAction(nameof(Details), new { id = created.Id });
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var a = await _audits.GetByIdAsync(id);
        return a is null ? NotFound() : View(a);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, SecurityAudit input)
    {
        if (id != input.Id) return BadRequest();
        ModelState.Remove(nameof(SecurityAudit.Reference));
        if (!ModelState.IsValid) return View(input);
        var ok = await _audits.UpdateHeaderAsync(input);
        if (!ok) return NotFound();
        TempData["Success"] = "Audit mis à jour.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> UpdateFinding(int auditId, int findingId, FindingResult result, string? observation)
    {
        await _audits.UpdateFindingAsync(findingId, result, observation ?? string.Empty);
        return RedirectToAction(nameof(Details), new { id = auditId });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Close(int id, string conclusion)
    {
        await _audits.CloseAsync(id, conclusion);
        TempData["Success"] = "Audit clôturé.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
