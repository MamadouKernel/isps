using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Zone à accès restreint au sens du Code ISPS (Part A §16.3, Part B §16.21) :
/// secteur sensible du terminal devant être identifié, signalé et protégé.
/// </summary>
public class RestrictedZone : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;          // ZAR-01

    [Required, MaxLength(150), Display(Name = "Désignation")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Niveau d'accès requis")]
    public ZoneAccessLevel AccessLevel { get; set; } = ZoneAccessLevel.Restreint;

    [Display(Name = "État")]
    public ZoneStatus Status { get; set; } = ZoneStatus.Active;

    [MaxLength(1000), Display(Name = "Description / Localisation")]
    public string Description { get; set; } = string.Empty;

    [MaxLength(2000), Display(Name = "Mesures de protection")]
    public string ProtectionMeasures { get; set; } = string.Empty;

    [MaxLength(2000), Display(Name = "Personnes / catégories autorisées")]
    public string AuthorizedPersonnel { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Responsable de zone")]
    public string ZoneManager { get; set; } = string.Empty;

    [Display(Name = "Escorte obligatoire")]
    public bool RequiresEscort { get; set; }

    [Display(Name = "Habilitation requise")]
    public bool RequiresClearance { get; set; }

    [Display(Name = "Vidéosurveillée (CCTV)")]
    public bool CctvMonitored { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp] public byte[]? RowVersion { get; set; }
}
