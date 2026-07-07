using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Journal de contrôle d'accès des véhicules / camions au terminal (ISPS Part A § 7).
/// </summary>
public class VehicleAccess : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Reference { get; set; } = string.Empty;     // ACC-2026-0001

    [Required, MaxLength(20), Display(Name = "Plaque d'immatriculation")]
    public string Plate { get; set; } = string.Empty;

    public VehicleType Type { get; set; } = VehicleType.Camion;
    public AccessDirection Direction { get; set; } = AccessDirection.Entree;
    public AccessControlResult Result { get; set; } = AccessControlResult.Autorise;

    [MaxLength(150), Display(Name = "Conducteur")]
    public string DriverName { get; set; } = string.Empty;

    [MaxLength(50), Display(Name = "N° pièce d'identité")]
    public string DriverIdNumber { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Transporteur / Société")]
    public string Carrier { get; set; } = string.Empty;

    [MaxLength(30), Display(Name = "N° conteneur")]
    public string ContainerNumber { get; set; } = string.Empty;

    [MaxLength(30), Display(Name = "N° de scellé")]
    public string SealNumber { get; set; } = string.Empty;

    [Display(Name = "Scellé vérifié conforme")]
    public bool SealVerified { get; set; }

    [MaxLength(80), Display(Name = "N° de booking / manifeste")]
    public string BookingReference { get; set; } = string.Empty;

    [Display(Name = "Fouille effectuée")]
    public bool Searched { get; set; }

    [MaxLength(150), Display(Name = "Agent contrôleur")]
    public string Controller { get; set; } = string.Empty;

    [MaxLength(50), Display(Name = "Portail")]
    public string Gate { get; set; } = string.Empty;

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    [MaxLength(1000), Display(Name = "Observations")]
    public string Notes { get; set; } = string.Empty;
}
