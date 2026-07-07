using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

public class Camera : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(30), Display(Name = "Identifiant")]
    public string Code { get; set; } = string.Empty;          // CCTV-001

    [Required, MaxLength(150), Display(Name = "Libellé")]
    public string Label { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Zone couverte")]
    public string Zone { get; set; } = string.Empty;

    public CameraType Type { get; set; } = CameraType.Fixe;
    public CameraStatus Status { get; set; } = CameraStatus.Operationnelle;

    [MaxLength(50), Display(Name = "Marque / Modèle")]
    public string Model { get; set; } = string.Empty;

    [MaxLength(80), Display(Name = "Adresse IP")]
    public string IpAddress { get; set; } = string.Empty;

    [Display(Name = "Dernière vérification")]
    public DateTime? LastCheckedAt { get; set; }

    [MaxLength(150), Display(Name = "Vérifiée par")]
    public string LastCheckedBy { get; set; } = string.Empty;

    [MaxLength(500), Display(Name = "Notes / Angle mort")]
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<CameraMaintenance> MaintenanceHistory { get; set; } = new();

    [Timestamp] public byte[]? RowVersion { get; set; }
}

public class CameraMaintenance
{
    public int Id { get; set; }

    public int CameraId { get; set; }
    public Camera? Camera { get; set; }

    [Required, MaxLength(200), Display(Name = "Intervention")]
    public string Action { get; set; } = string.Empty;

    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(150), Display(Name = "Effectuée par")]
    public string PerformedBy { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public CameraStatus ResultingStatus { get; set; } = CameraStatus.Operationnelle;
}
