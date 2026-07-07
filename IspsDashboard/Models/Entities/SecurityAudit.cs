using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Audit de sûreté (interne/externe) basé sur des points de contrôle ISPS Part B.
/// </summary>
public class SecurityAudit : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Reference { get; set; } = string.Empty;     // AUD-2026-0001

    [Required, MaxLength(200), Display(Name = "Intitulé")]
    public string Title { get; set; } = string.Empty;

    public AuditType Type { get; set; } = AuditType.Interne;
    public AuditStatus Status { get; set; } = AuditStatus.Planifie;

    [Required, Display(Name = "Date prévue")]
    public DateTime ScheduledDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    [MaxLength(150), Display(Name = "Auditeur")]
    public string Auditor { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Périmètre audité")]
    public string Scope { get; set; } = string.Empty;

    [MaxLength(2000), Display(Name = "Conclusion / Synthèse")]
    public string Conclusion { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<AuditFinding> Findings { get; set; } = new();

    [Timestamp] public byte[]? RowVersion { get; set; }

    public int ConformityScore()
    {
        var evaluated = Findings.Where(f => f.Result != FindingResult.NonApplicable).ToList();
        if (evaluated.Count == 0) return 100;
        var conform = evaluated.Count(f => f.Result == FindingResult.Conforme);
        return (int)Math.Round((double)conform / evaluated.Count * 100);
    }
}

public class AuditFinding
{
    public int Id { get; set; }

    public int SecurityAuditId { get; set; }
    public SecurityAudit? Audit { get; set; }

    public int ItemNumber { get; set; }

    [Required, MaxLength(500), Display(Name = "Point de contrôle")]
    public string CheckItem { get; set; } = string.Empty;

    public FindingResult Result { get; set; } = FindingResult.Conforme;

    [MaxLength(2000), Display(Name = "Constat / Observation")]
    public string Observation { get; set; } = string.Empty;
}
