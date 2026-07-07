using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Journal d'un changement de niveau MARSEC (ISPS 1, 2 ou 3).
/// Tracé pour audit ISPS — chaque transition doit être justifiée.
/// </summary>
public class MarsecLevelChange
{
    public int Id { get; set; }

    [Range(1, 3)] public int FromLevel { get; set; }
    [Range(1, 3)] public int ToLevel { get; set; }

    [Required, MaxLength(1000), Display(Name = "Motif")]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(200), Display(Name = "Source de la décision")]
    public string DecisionSource { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Décidé par")]
    public string DecidedBy { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public string? ChangedById { get; set; }

    public List<MarsecChecklistItem> ChecklistItems { get; set; } = new();
}

/// <summary>
/// Élément de checklist exécuté lors d'un changement de niveau MARSEC.
/// Chaque niveau impose des actions normées (renforcement patrouilles, fouilles, etc.).
/// </summary>
public class MarsecChecklistItem
{
    public int Id { get; set; }

    public int MarsecLevelChangeId { get; set; }
    public MarsecLevelChange? MarsecLevelChange { get; set; }

    [Required, MaxLength(300)]
    public string Action { get; set; } = string.Empty;

    public bool Completed { get; set; }

    public DateTime? CompletedAt { get; set; }

    [MaxLength(150)]
    public string CompletedBy { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
