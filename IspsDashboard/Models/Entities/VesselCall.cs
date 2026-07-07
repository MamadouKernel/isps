using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Escale d'un navire au terminal — concentre les informations sûreté du navire et de l'escale.
/// </summary>
public class VesselCall : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Reference { get; set; } = string.Empty;     // ESC-2026-0001

    [Required, MaxLength(150), Display(Name = "Nom du navire")]
    public string VesselName { get; set; } = string.Empty;

    [MaxLength(15), Display(Name = "N° IMO")]
    public string ImoNumber { get; set; } = string.Empty;

    [MaxLength(20), Display(Name = "Indicatif d'appel")]
    public string CallSign { get; set; } = string.Empty;

    [MaxLength(80), Display(Name = "Pavillon")]
    public string Flag { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Compagnie")]
    public string Operator { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "CSO (Company Security Officer)")]
    public string Cso { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "SSO (Ship Security Officer)")]
    public string Sso { get; set; } = string.Empty;

    [Range(1, 3), Display(Name = "Niveau ISPS du navire")]
    public int ShipIspsLevel { get; set; } = 1;

    [Required, Display(Name = "ETA (arrivée prévue)")]
    public DateTime Eta { get; set; }

    [Display(Name = "ETD (départ prévu)")]
    public DateTime? Etd { get; set; }

    public DateTime? ActualArrival { get; set; }
    public DateTime? ActualDeparture { get; set; }

    [MaxLength(50), Display(Name = "Poste à quai")]
    public string Berth { get; set; } = string.Empty;

    public VesselCallStatus Status { get; set; } = VesselCallStatus.Annonce;

    [MaxLength(2000), Display(Name = "Notes / Observations sûreté")]
    public string SecurityNotes { get; set; } = string.Empty;

    [MaxLength(2000), Display(Name = "10 derniers ports d'escale")]
    public string LastTenPorts { get; set; } = string.Empty;

    public int CrewCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<DeclarationOfSecurity> Declarations { get; set; } = new();

    [Timestamp] public byte[]? RowVersion { get; set; }
}

/// <summary>
/// Déclaration de Sûreté (DoS) — document formel entre le navire et l'installation portuaire
/// requis lorsque les niveaux ISPS du navire et du port diffèrent, ou sur demande.
/// </summary>
public class DeclarationOfSecurity
{
    public int Id { get; set; }

    public int VesselCallId { get; set; }
    public VesselCall? VesselCall { get; set; }

    [Required, MaxLength(20)]
    public string Reference { get; set; } = string.Empty;     // DOS-2026-0001

    public DosStatus Status { get; set; } = DosStatus.Brouillon;

    [Range(1, 3), Display(Name = "Niveau port")]
    public int PortLevel { get; set; }

    [Range(1, 3), Display(Name = "Niveau navire")]
    public int ShipLevel { get; set; }

    [Required, Display(Name = "Date d'effet")]
    public DateTime EffectiveFrom { get; set; }

    [Display(Name = "Date de fin de validité")]
    public DateTime? EffectiveTo { get; set; }

    [MaxLength(150), Display(Name = "Signé par (PFSO)")]
    public string SignedByPfso { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Signé par (SSO / Commandant)")]
    public string SignedByShip { get; set; } = string.Empty;

    public DateTime? SignedAt { get; set; }

    [MaxLength(4000), Display(Name = "Mesures convenues")]
    public string AgreedMeasures { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp] public byte[]? RowVersion { get; set; }
}
