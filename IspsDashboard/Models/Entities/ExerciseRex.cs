using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Retour d'expérience structuré post-exercice (ISPS Part B § 18.6).
/// 5 sections obligatoires pour conformité audit.
/// </summary>
public class ExerciseRex : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    public int ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    [MaxLength(4000), Display(Name = "Déroulé de l'exercice")]
    public string Sequence { get; set; } = string.Empty;

    [MaxLength(4000), Display(Name = "Points positifs")]
    public string PositivePoints { get; set; } = string.Empty;

    [MaxLength(4000), Display(Name = "Points à améliorer")]
    public string ImprovementPoints { get; set; } = string.Empty;

    [MaxLength(4000), Display(Name = "Actions correctives décidées")]
    public string CorrectiveActions { get; set; } = string.Empty;

    [MaxLength(4000), Display(Name = "Suivi & échéances")]
    public string FollowUp { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Rédigé par")]
    public string WrittenBy { get; set; } = string.Empty;

    public DateTime WrittenAt { get; set; } = DateTime.UtcNow;

    [Timestamp] public byte[]? RowVersion { get; set; }
}
