using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class MarsecService : IMarsecService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;
    private readonly IDashboardService _dashboard;

    public MarsecService(ApplicationDbContext db, IAuditService audit, IDashboardService dashboard)
    {
        _db = db;
        _audit = audit;
        _dashboard = dashboard;
    }

    public async Task<int> GetCurrentLevelAsync()
    {
        var settings = await _db.DashboardSettings.AsNoTracking().FirstOrDefaultAsync();
        return settings?.IspsLevel ?? 1;
    }

    public async Task<MarsecLevelChange> RequestChangeAsync(
        int newLevel, string reason, string decisionSource, string decidedBy, string? decidedById)
    {
        if (newLevel is < 1 or > 3)
            throw new ArgumentOutOfRangeException(nameof(newLevel), "Le niveau MARSEC doit être 1, 2 ou 3.");

        var settings = await _db.DashboardSettings.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Paramètres dashboard introuvables.");

        var fromLevel = settings.IspsLevel;

        var change = new MarsecLevelChange
        {
            FromLevel = fromLevel,
            ToLevel = newLevel,
            Reason = reason.Trim(),
            DecisionSource = decisionSource?.Trim() ?? string.Empty,
            DecidedBy = decidedBy?.Trim() ?? string.Empty,
            ChangedAt = DateTime.UtcNow,
            ChangedById = decidedById,
            ChecklistItems = GetChecklistTemplate(newLevel)
                .Select(action => new MarsecChecklistItem { Action = action })
                .ToList()
        };

        settings.IspsLevel = newLevel;
        settings.UpdatedAt = DateTime.UtcNow;
        settings.UpdatedById = decidedById;

        _db.MarsecLevelChanges.Add(change);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("ChangeMarsecLevel",
            $"Niveau {fromLevel} → {newLevel}. Motif: {reason}");

        _dashboard.InvalidateCache();
        return change;
    }

    public async Task<IReadOnlyList<MarsecLevelChange>> GetHistoryAsync(int take = 50)
        => await _db.MarsecLevelChanges.AsNoTracking()
            .Include(c => c.ChecklistItems)
            .OrderByDescending(c => c.ChangedAt)
            .Take(take)
            .ToListAsync();

    public async Task<bool> CompleteChecklistItemAsync(int itemId, string completedBy, string? notes)
    {
        var item = await _db.MarsecChecklistItems.FirstOrDefaultAsync(i => i.Id == itemId);
        if (item is null) return false;
        item.Completed = true;
        item.CompletedAt = DateTime.UtcNow;
        item.CompletedBy = completedBy?.Trim() ?? string.Empty;
        item.Notes = notes?.Trim();
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CompleteMarsecChecklist", $"#{itemId} — {item.Action}");
        return true;
    }

    /// <summary>Modèle d'actions ISPS pour chaque niveau. À adapter selon le PFSP local.</summary>
    public IReadOnlyList<string> GetChecklistTemplate(int targetLevel) => targetLevel switch
    {
        1 => new[]
        {
            "Reprendre la fréquence normale des patrouilles",
            "Désarmer les contrôles renforcés véhicules",
            "Informer le personnel du retour à la normale",
            "Tracer la décision dans le PFSP",
            "Notifier le PAA / autorités portuaires du changement"
        },
        2 => new[]
        {
            "Renforcer les patrouilles périmétriques (fréquence doublée)",
            "Activer le contrôle systématique des véhicules entrants",
            "Renforcer la fouille des camions et conteneurs (échantillonnage 30%)",
            "Activer toutes les caméras et alarmes en mode haute sensibilité",
            "Tenir un briefing exceptionnel des équipes",
            "Restreindre l'accès des visiteurs non essentiels",
            "Notifier le PAA / Gendarmerie maritime",
            "Préparer la cellule de crise (stand-by)",
            "Vérifier les habilitations des personnels présents",
            "Tracer la décision dans le PFSP"
        },
        3 => new[]
        {
            "Activer la cellule de crise immédiatement",
            "Verrouiller les accès non essentiels",
            "Évacuer les visiteurs et personnels non essentiels",
            "Coordination avec autorités (PAA, Gendarmerie maritime, Préfet maritime)",
            "Fouille systématique 100% véhicules et personnes",
            "Activation des secours externes (SAMU, pompiers, forces de l'ordre)",
            "Communication de crise vers la direction et autorités",
            "Préparer la consignation des navires en escale (suspension des opérations)",
            "Documenter chaque décision en temps réel",
            "Tracer la décision dans le PFSP"
        },
        _ => Array.Empty<string>()
    };
}
