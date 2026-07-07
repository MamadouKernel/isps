using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Briefing de prise / fin de poste pour assurer la passation entre équipes.
/// </summary>
public class ShiftBriefing : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, Display(Name = "Date de vacation")]
    public DateTime ShiftDate { get; set; }

    public ShiftSlot Slot { get; set; }

    [MaxLength(150), Display(Name = "Agent sortant")]
    public string OutgoingAgent { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Agent entrant")]
    public string IncomingAgent { get; set; } = string.Empty;

    [Range(1, 3), Display(Name = "Niveau MARSEC en cours")]
    public int CurrentMarsecLevel { get; set; } = 1;

    [MaxLength(4000), Display(Name = "Événements de la vacation écoulée")]
    public string EventsSummary { get; set; } = string.Empty;

    [MaxLength(4000), Display(Name = "Points d'attention pour la suite")]
    public string AttentionPoints { get; set; } = string.Empty;

    [MaxLength(4000), Display(Name = "Consignes du jour")]
    public string StandingOrders { get; set; } = string.Empty;

    public bool AcknowledgedByIncoming { get; set; }
    public DateTime? AcknowledgedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp] public byte[]? RowVersion { get; set; }
}
