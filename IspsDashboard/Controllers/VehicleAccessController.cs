using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize]
public class VehicleAccessController : Controller
{
    private readonly IVehicleAccessService _service;
    private readonly IExportService _export;
    private readonly ISoftDeleteService _softDelete;

    public VehicleAccessController(IVehicleAccessService service, IExportService export, ISoftDeleteService softDelete)
    {
        _service = service;
        _export = export;
        _softDelete = softDelete;
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<VehicleAccess>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "Mouvement supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    private async Task<TableData> BuildExportAsync(AccessDirection? direction, DateTime? from, DateTime? to, string? query)
    {
        var rows = await _service.SearchAsync(direction, from, to, query);
        var headers = new[] { "Référence", "Date/heure", "Plaque", "Type", "Sens", "Résultat", "Conducteur", "Transporteur", "Conteneur", "Scellé", "Scellé OK", "Fouille", "Portail", "Agent" };
        var data = rows.Select(v => (IReadOnlyList<string>)new[]
        {
            v.Reference, v.OccurredAt.ToString("dd/MM/yyyy HH:mm"), v.Plate, v.Type.ToString(), v.Direction.ToString(),
            v.Result.ToString(), v.DriverName, v.Carrier, v.ContainerNumber, v.SealNumber,
            v.SealVerified ? "Oui" : "Non", v.Searched ? "Oui" : "Non", v.Gate, v.Controller
        }).ToList();
        return new TableData("Mouvements véhicules & camions", headers, data);
    }

    public async Task<IActionResult> ExportExcel(AccessDirection? direction, DateTime? from, DateTime? to, string? query)
        => File(_export.ToExcel(await BuildExportAsync(direction, from, to, query)),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Mouvements-vehicules-{DateTime.Now:yyyyMMdd}.xlsx");

    public async Task<IActionResult> ExportPdf(AccessDirection? direction, DateTime? from, DateTime? to, string? query)
        => File(_export.ToPdf(await BuildExportAsync(direction, from, to, query)), "application/pdf", $"Mouvements-vehicules-{DateTime.Now:yyyyMMdd}.pdf");

    public async Task<IActionResult> Index(AccessDirection? direction, DateTime? from, DateTime? to, string? query)
    {
        ViewBag.Today = await _service.GetTodayAsync();
        ViewBag.Results = await _service.SearchAsync(direction, from, to, query);
        ViewBag.Direction = direction;
        ViewBag.Query = query;
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var v = await _service.GetByIdAsync(id);
        return v is null ? NotFound() : View(v);
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id)
    {
        var v = await _service.GetByIdAsync(id);
        return v is null ? NotFound() : View(v);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int id, VehicleAccess input)
    {
        if (id != input.Id) return BadRequest();
        ModelState.Remove(nameof(VehicleAccess.Reference));
        if (!ModelState.IsValid) return View(input);
        var ok = await _service.UpdateAsync(input);
        if (!ok) return NotFound();
        TempData["Success"] = "Mouvement mis à jour.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "RequireEditor")]
    public IActionResult Create() => View(new VehicleAccess { OccurredAt = DateTime.Now });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Create(VehicleAccess input)
    {
        ModelState.Remove(nameof(VehicleAccess.Reference)); // générée côté serveur
        if (!ModelState.IsValid) return View(input);
        if (string.IsNullOrEmpty(input.Controller)) input.Controller = User.Identity?.Name ?? "system";
        var created = await _service.CreateAsync(input);
        TempData["Success"] = $"Contrôle d'accès enregistré : {created.Reference}";
        return RedirectToAction(nameof(Index));
    }
}
