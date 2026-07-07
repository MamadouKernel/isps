using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

public class Visitor : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Reference { get; set; } = string.Empty;   // VIS-2026-0001

    [Required, MaxLength(150), Display(Name = "Nom complet")]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Entreprise")]
    public string Company { get; set; } = string.Empty;

    [MaxLength(50), Display(Name = "N° pièce d'identité")]
    public string IdDocumentNumber { get; set; } = string.Empty;

    [MaxLength(30), Display(Name = "Téléphone")]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(20), Display(Name = "Plaque véhicule")]
    public string VehiclePlate { get; set; } = string.Empty;

    [Required, Display(Name = "Date prévue d'arrivée")]
    public DateTime ScheduledArrival { get; set; }

    [Display(Name = "Date prévue de sortie")]
    public DateTime? ScheduledDeparture { get; set; }

    [Required, MaxLength(150), Display(Name = "Motif de visite")]
    public string Purpose { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Personne visitée")]
    public string Host { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Agent escorte")]
    public string EscortedBy { get; set; } = string.Empty;

    [MaxLength(20), Display(Name = "Badge délivré")]
    public string BadgeIssued { get; set; } = string.Empty;

    public VisitorStatus Status { get; set; } = VisitorStatus.PreEnregistre;

    public DateTime? CheckInAt { get; set; }
    public DateTime? CheckOutAt { get; set; }

    [MaxLength(150)] public string CheckedInBy { get; set; } = string.Empty;
    [MaxLength(150)] public string CheckedOutBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(1000), Display(Name = "Notes")]
    public string Notes { get; set; } = string.Empty;

    [Timestamp] public byte[]? RowVersion { get; set; }
}
