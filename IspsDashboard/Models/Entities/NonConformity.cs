using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Non-conformité détectée (audit, inspection, incident…) avec plan d'action CAPA.
/// </summary>
public class NonConformity : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Reference { get; set; } = string.Empty;   // NC-2026-0001

    [Required, MaxLength(200), Display(Name = "Intitulé")]
    public string Title { get; set; } = string.Empty;

    public NonConformitySource Source { get; set; }
    public NonConformityStatus Status { get; set; } = NonConformityStatus.Identifiee;

    [Required, MaxLength(2000), Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [MaxLength(2000), Display(Name = "Action corrective (CAPA)")]
    public string CorrectiveAction { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Responsable du traitement")]
    public string Owner { get; set; } = string.Empty;

    [Required, Display(Name = "Date d'identification")]
    public DateTime IdentifiedAt { get; set; }

    [Display(Name = "Échéance de levée")]
    public DateTime? DueDate { get; set; }

    [Display(Name = "Date de levée")]
    public DateTime? ClosedAt { get; set; }

    [MaxLength(1000), Display(Name = "Preuve de clôture")]
    public string ClosureEvidence { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp] public byte[]? RowVersion { get; set; }
}
