using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Journal d'un passage d'un agent à un checkpoint donné.
/// </summary>
public class PatrolScan
{
    public int Id { get; set; }

    public int CheckpointId { get; set; }
    public Checkpoint? Checkpoint { get; set; }

    public int? AgentId { get; set; }
    public Agent? Agent { get; set; }

    [Required, MaxLength(150)]
    public string AgentLabel { get; set; } = string.Empty;   // dénormalisé pour historique

    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Observations { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    /// <summary>Anomalie déclarée à ce passage (incendie, intrusion, dégradation…).</summary>
    [MaxLength(200)]
    public string? AnomalyType { get; set; }
}
