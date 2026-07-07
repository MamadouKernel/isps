using IspsDashboard.Models.Entities;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IspsDashboard.Controllers;

[Authorize(Policy = "RequireAdmin")]
public class TrashController : Controller
{
    private readonly ISoftDeleteService _softDelete;

    public TrashController(ISoftDeleteService softDelete) => _softDelete = softDelete;

    public async Task<IActionResult> Index()
    {
        var entries = new List<TrashEntry>();
        await AddAsync<AccessPass>(entries, "Laissez-passer", p => $"{p.Reference} — {p.FullName}");
        await AddAsync<VehicleAccess>(entries, "Mouvement véhicule", v => $"{v.Reference} — {v.Plate}");
        await AddAsync<Visitor>(entries, "Visiteur", v => $"{v.Reference} — {v.FullName}");
        await AddAsync<Incident>(entries, "Incident", i => $"{i.Reference} — {i.Title}");
        await AddAsync<RestrictedZone>(entries, "Zone restreinte", z => $"{z.Code} — {z.Name}");
        await AddAsync<VesselCall>(entries, "Escale navire", v => $"{v.Reference} — {v.VesselName}");
        await AddAsync<ShiftBriefing>(entries, "Briefing", b => $"{b.ShiftDate:dd/MM/yyyy} / {b.Slot}");
        await AddAsync<Agent>(entries, "Agent", a => $"#{a.Position} — {a.Name}");
        await AddAsync<Habilitation>(entries, "Habilitation", h => h.Title);
        await AddAsync<Camera>(entries, "Caméra", c => $"{c.Code} — {c.Label}");
        await AddAsync<ExternalContact>(entries, "Contact externe", c => c.Name);
        await AddAsync<NonConformity>(entries, "Non-conformité", n => $"{n.Reference} — {n.Title}");
        await AddAsync<SecurityAudit>(entries, "Audit", a => $"{a.Reference} — {a.Title}");
        await AddAsync<ExerciseRex>(entries, "REX", r => $"Exercice #{r.ExerciseId}");
        await AddAsync<SecurityDocument>(entries, "Document", d => $"{d.Title} v{d.Version}");
        await AddAsync<Exercise>(entries, "Exercice", e => e.Name);
        await AddAsync<Checkpoint>(entries, "Checkpoint", c => $"{c.Code} — {c.Label}");
        await AddAsync<KpiCard>(entries, "Carte KPI", k => k.Label);
        await AddAsync<KpiTableRow>(entries, "Indicateur tableau", k => k.Indicator);
        await AddAsync<Gauge>(entries, "Jauge", g => g.Label);
        await AddAsync<ProgressBar>(entries, "Barre de progression", b => b.Label);

        return View(new TrashViewModel { Entries = entries.OrderByDescending(e => e.DeletedAt).ToList() });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(string type, int id)
    {
        var ok = type switch
        {
            nameof(AccessPass) => await _softDelete.RestoreAsync<AccessPass>(id),
            nameof(VehicleAccess) => await _softDelete.RestoreAsync<VehicleAccess>(id),
            nameof(Visitor) => await _softDelete.RestoreAsync<Visitor>(id),
            nameof(Incident) => await _softDelete.RestoreAsync<Incident>(id),
            nameof(RestrictedZone) => await _softDelete.RestoreAsync<RestrictedZone>(id),
            nameof(VesselCall) => await _softDelete.RestoreAsync<VesselCall>(id),
            nameof(ShiftBriefing) => await _softDelete.RestoreAsync<ShiftBriefing>(id),
            nameof(Agent) => await _softDelete.RestoreAsync<Agent>(id),
            nameof(Habilitation) => await _softDelete.RestoreAsync<Habilitation>(id),
            nameof(Camera) => await _softDelete.RestoreAsync<Camera>(id),
            nameof(ExternalContact) => await _softDelete.RestoreAsync<ExternalContact>(id),
            nameof(NonConformity) => await _softDelete.RestoreAsync<NonConformity>(id),
            nameof(SecurityAudit) => await _softDelete.RestoreAsync<SecurityAudit>(id),
            nameof(ExerciseRex) => await _softDelete.RestoreAsync<ExerciseRex>(id),
            nameof(SecurityDocument) => await _softDelete.RestoreAsync<SecurityDocument>(id),
            nameof(Exercise) => await _softDelete.RestoreAsync<Exercise>(id),
            nameof(Checkpoint) => await _softDelete.RestoreAsync<Checkpoint>(id),
            nameof(KpiCard) => await _softDelete.RestoreAsync<KpiCard>(id),
            nameof(KpiTableRow) => await _softDelete.RestoreAsync<KpiTableRow>(id),
            nameof(Gauge) => await _softDelete.RestoreAsync<Gauge>(id),
            nameof(ProgressBar) => await _softDelete.RestoreAsync<ProgressBar>(id),
            _ => false
        };
        TempData[ok ? "Success" : "Error"] = ok ? "Élément restauré." : "Restauration impossible.";
        return RedirectToAction(nameof(Index));
    }

    private async Task AddAsync<T>(List<TrashEntry> list, string label, Func<T, string> describe)
        where T : class, ISoftDeletable
    {
        foreach (var e in await _softDelete.GetDeletedAsync<T>())
        {
            var id = (int)(typeof(T).GetProperty("Id")!.GetValue(e) ?? 0);
            list.Add(new TrashEntry(typeof(T).Name, label, id, describe(e), e.DeletedAt, e.DeletedById));
        }
    }
}
