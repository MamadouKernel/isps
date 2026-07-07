using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Point de contrôle physique d'une ronde (portail, périmètre, zone restreinte).
/// L'agent scanne le QR code pour valider son passage.
/// </summary>
public class Checkpoint : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;          // CP-001

    [Required, MaxLength(120), Display(Name = "Libellé")]
    public string Label { get; set; } = string.Empty;

    [MaxLength(120), Display(Name = "Zone")]
    public string Zone { get; set; } = string.Empty;

    /// <summary>Délai maximum entre deux passages (minutes). Aide à détecter les patrouilles oubliées.</summary>
    [Display(Name = "Fréquence cible (min)")]
    public int TargetIntervalMinutes { get; set; } = 240;

    [Display(Name = "Latitude (optionnelle)")]
    public double? Latitude { get; set; }
    [Display(Name = "Longitude (optionnelle)")]
    public double? Longitude { get; set; }

    [Display(Name = "Actif")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
