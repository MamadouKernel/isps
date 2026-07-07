using IspsDashboard.Data;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

/// <summary>
/// Construit la liste consolidée des alertes affichées dans le centre de notifications (cloche).
/// </summary>
public sealed class AlertService : IAlertService
{
    private readonly ApplicationDbContext _db;
    private readonly IClock _clock;
    private readonly IExerciseService _exercises;
    private readonly IHabilitationService _habilitations;
    private readonly IPatrolService _patrols;
    private readonly IAccessPassService _passes;

    public AlertService(
        ApplicationDbContext db,
        IClock clock,
        IExerciseService exercises,
        IHabilitationService habilitations,
        IPatrolService patrols,
        IAccessPassService passes)
    {
        _db = db;
        _clock = clock;
        _exercises = exercises;
        _habilitations = habilitations;
        _patrols = patrols;
        _passes = passes;
    }

    public async Task<AlertCenterViewModel> BuildAsync()
    {
        var alerts = new List<AlertItem>();
        var today = _clock.Today;

        // KPI critiques
        var criticalKpis = await _db.KpiTableRows.AsNoTracking()
            .Where(k => k.Status == KpiStatus.Critique).ToListAsync();
        foreach (var k in criticalKpis)
        {
            alerts.Add(new AlertItem
            {
                Level = AlertLevel.Critical,
                Icon = "🔴",
                Title = $"KPI critique : {k.Indicator}",
                Detail = $"{k.Category} — valeur {k.CurrentValue} (objectif {k.Target})",
                Url = "/Dashboard"
            });
        }

        // Incidents ouverts
        var openIncidents = await _db.Incidents.AsNoTracking()
            .Where(i => i.Status != IncidentStatus.Clos)
            .OrderByDescending(i => i.OccurredAt).Take(5).ToListAsync();
        foreach (var i in openIncidents)
        {
            alerts.Add(new AlertItem
            {
                Level = i.Severity >= IncidentSeverity.Majeur ? AlertLevel.Critical : AlertLevel.Warning,
                Icon = "🚨",
                Title = $"Incident {i.Reference} non clos",
                Detail = i.Title,
                Url = $"/Incidents/Details/{i.Id}"
            });
        }

        // Habilitations expirées / à renouveler
        foreach (var h in await _habilitations.GetExpiredAsync())
        {
            alerts.Add(new AlertItem
            {
                Level = AlertLevel.Critical,
                Icon = "🎓",
                Title = "Habilitation expirée",
                Detail = $"{h.Agent?.Name} — {h.Title} (depuis {Math.Abs(_habilitations.DaysToExpiry(h))} j)",
                Url = $"/Personnel/Details/{h.AgentId}"
            });
        }
        foreach (var h in (await _habilitations.GetExpiringSoonAsync(30)))
        {
            alerts.Add(new AlertItem
            {
                Level = AlertLevel.Warning,
                Icon = "🎓",
                Title = "Habilitation à renouveler",
                Detail = $"{h.Agent?.Name} — {h.Title} (J-{_habilitations.DaysToExpiry(h)})",
                Url = $"/Personnel/Details/{h.AgentId}"
            });
        }

        // Patrouilles en retard
        foreach (var o in await _patrols.GetOverdueAsync())
        {
            alerts.Add(new AlertItem
            {
                Level = AlertLevel.Warning,
                Icon = "🚶",
                Title = $"Patrouille en retard : {o.Checkpoint.Code}",
                Detail = $"{o.Checkpoint.Label} — cible {o.Checkpoint.TargetIntervalMinutes} min",
                Url = "/Patrols"
            });
        }

        // Exercices J-7
        var horizon = today.AddDays(7);
        var upcoming = await _db.Exercises.AsNoTracking()
            .Where(e => e.Status == ExerciseStatus.Planifie && e.PlannedDate.Date <= horizon && e.PlannedDate.Date >= today)
            .OrderBy(e => e.PlannedDate).ToListAsync();
        foreach (var e in upcoming)
        {
            alerts.Add(new AlertItem
            {
                Level = AlertLevel.Warning,
                Icon = "⏰",
                Title = $"Exercice imminent : {e.Name}",
                Detail = $"Prévu le {e.PlannedDate:dd/MM/yyyy} (J-{(int)(e.PlannedDate.Date - today).TotalDays})",
                Url = "/Dashboard"
            });
        }

        // Laissez-passer expirant sous 7 j
        var passes = await _passes.SearchAsync(null, null, null);
        foreach (var p in passes.Where(x => !x.Revoked))
        {
            var d = _passes.DaysToExpiry(p);
            if (d is >= 0 and <= 7)
            {
                alerts.Add(new AlertItem
                {
                    Level = AlertLevel.Info,
                    Icon = "🪪",
                    Title = $"Laissez-passer expirant : {p.Reference}",
                    Detail = $"{p.FullName} — expire le {p.EndDate:dd/MM/yyyy} (J-{d})",
                    Url = $"/AccessPass/Edit/{p.Id}"
                });
            }
        }

        // Non-conformités en retard d'échéance
        var overdueNc = await _db.NonConformities.AsNoTracking()
            .Where(n => n.Status != NonConformityStatus.Levee && n.Status != NonConformityStatus.Rejetee
                        && n.DueDate != null && n.DueDate < today).ToListAsync();
        foreach (var n in overdueNc)
        {
            alerts.Add(new AlertItem
            {
                Level = AlertLevel.Critical,
                Icon = "❗",
                Title = $"Non-conformité en retard : {n.Reference}",
                Detail = $"{n.Title} — échéance dépassée ({n.DueDate:dd/MM/yyyy})",
                Url = $"/NonConformities/Details/{n.Id}"
            });
        }

        var ordered = alerts
            .OrderByDescending(a => a.Level)
            .ToList();

        return new AlertCenterViewModel { Alerts = ordered };
    }
}
