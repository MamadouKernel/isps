using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class AuditCampaignService : IAuditCampaignService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public AuditCampaignService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<IReadOnlyList<SecurityAudit>> GetAllAsync()
        => await _db.SecurityAudits.AsNoTracking()
            .Include(a => a.Findings)
            .OrderByDescending(a => a.ScheduledDate)
            .ToListAsync();

    public Task<SecurityAudit?> GetByIdAsync(int id)
        => _db.SecurityAudits.Include(a => a.Findings.OrderBy(f => f.ItemNumber))
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<SecurityAudit> CreateAsync(SecurityAudit input, bool seedChecklist)
    {
        input.CreatedAt = DateTime.UtcNow;
        input.Auditor = input.Auditor?.Trim() ?? string.Empty;
        input.Scope = input.Scope?.Trim() ?? string.Empty;
        if (seedChecklist)
        {
            var template = GetIspsChecklistTemplate();
            input.Findings = template.Select((item, idx) => new AuditFinding
            {
                ItemNumber = idx + 1,
                CheckItem = item,
                Result = FindingResult.Conforme
            }).ToList();
        }
        _db.SecurityAudits.Add(input);
        await ReferenceGenerator.SaveWithUniqueReferenceAsync(_db, r => input.Reference = r, () => NextReference(input.ScheduledDate.Year));
        await _audit.LogAsync("CreateAudit", $"{input.Reference} — {input.Title}");
        return input;
    }

    public async Task<bool> UpdateHeaderAsync(SecurityAudit input)
    {
        var e = await _db.SecurityAudits.FirstOrDefaultAsync(a => a.Id == input.Id);
        if (e is null) return false;
        e.Title = input.Title.Trim();
        e.Type = input.Type;
        e.ScheduledDate = input.ScheduledDate;
        e.Auditor = input.Auditor?.Trim() ?? string.Empty;
        e.Scope = input.Scope?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateAudit", e.Reference);
        return true;
    }

    public async Task<bool> UpdateFindingAsync(int findingId, FindingResult result, string observation)
    {
        var finding = await _db.AuditFindings.FirstOrDefaultAsync(f => f.Id == findingId);
        if (finding is null) return false;
        finding.Result = result;
        finding.Observation = observation?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CloseAsync(int id, string conclusion)
    {
        var audit = await _db.SecurityAudits.FirstOrDefaultAsync(a => a.Id == id);
        if (audit is null) return false;
        audit.Status = AuditStatus.Cloture;
        audit.CompletedDate = DateTime.UtcNow;
        audit.Conclusion = conclusion?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CloseAudit", audit.Reference);
        return true;
    }

    public string NextReference(int year)
    {
        var prefix = $"AUD-{year}-";
        var last = _db.SecurityAudits
            .IgnoreQueryFilters()
            .Where(a => a.Reference.StartsWith(prefix))
            .AsEnumerable()
            .Select(a => int.TryParse(a.Reference[prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return prefix + (last + 1).ToString("D4");
    }

    /// <summary>Points de contrôle ISPS Part B (extrait représentatif — à adapter au PFSP).</summary>
    public IReadOnlyList<string> GetIspsChecklistTemplate() => new[]
    {
        "Le PFSP est à jour et approuvé par l'autorité compétente",
        "Le PFSO (Responsable Sûreté) est désigné et formé",
        "Les agents de sûreté sont à jour de leur formation ISPS",
        "Les habilitations du personnel sont valides",
        "Le contrôle d'accès au terminal est effectif (badges, registres)",
        "Les véhicules et camions sont contrôlés à l'entrée",
        "Les visiteurs sont enregistrés et escortés",
        "La clôture périmétrique est intègre sur tout le périmètre",
        "L'éclairage périmétrique est opérationnel",
        "Les caméras CCTV couvrent les zones sensibles et fonctionnent",
        "Les alarmes d'intrusion sont opérationnelles et testées",
        "Les patrouilles sont effectuées selon la fréquence prévue",
        "Les exercices et entraînements sont réalisés selon le calendrier",
        "Les REX d'exercices sont rédigés et exploités",
        "Le registre des incidents de sûreté est tenu à jour",
        "Les non-conformités précédentes sont levées",
        "Les Déclarations de Sûreté (DoS) sont établies quand requis",
        "Les niveaux MARSEC sont correctement appliqués et tracés",
        "Les contacts d'urgence externes sont à jour et accessibles",
        "Les zones d'accès restreint sont signalées et protégées"
    };
}
