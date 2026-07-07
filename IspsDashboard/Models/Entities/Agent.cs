using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Représente un membre du personnel sûreté (agent de garde).
/// Position 1..12 = créneau de garde nominal du terminal.
/// </summary>
public class Agent : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(100), Display(Name = "Nom complet")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Présent")]
    public bool IsPresent { get; set; } = true;

    [Display(Name = "Position de garde")]
    public int Position { get; set; }

    [MaxLength(30), Display(Name = "Numéro de badge")]
    public string BadgeNumber { get; set; } = string.Empty;

    [MaxLength(30), Display(Name = "Téléphone")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, MaxLength(150), Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Date d'embauche")]
    public DateTime? HiredAt { get; set; }

    [MaxLength(80), Display(Name = "Fonction")]
    public string Role { get; set; } = "Agent de sûreté";

    [MaxLength(500), Display(Name = "Photo (chemin)")]
    public string PhotoPath { get; set; } = string.Empty;

    [MaxLength(1000), Display(Name = "Notes internes")]
    public string Notes { get; set; } = string.Empty;

    public List<Habilitation> Habilitations { get; set; } = new();

    [Timestamp] public byte[]? RowVersion { get; set; }
}
