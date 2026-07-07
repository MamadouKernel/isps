using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Laissez-passer d'accès au terminal (personne ou véhicule), de validité
/// journalière, trimestrielle ou annuelle.
/// </summary>
public class AccessPass : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(20)]
    public string Reference { get; set; } = string.Empty;     // LP-2026-0001

    [Display(Name = "Type d'accès")]
    public AccessPassType Type { get; set; } = AccessPassType.Journalier;

    [Display(Name = "Catégorie")]
    public AccessPassCategory Category { get; set; } = AccessPassCategory.Personne;

    [Required, MaxLength(100), Display(Name = "Nom")]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(100), Display(Name = "Prénom")]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(40), Display(Name = "Contact")]
    public string Contact { get; set; } = string.Empty;

    [MaxLength(50), Display(Name = "Matricule")]
    public string Matricule { get; set; } = string.Empty;

    [EmailAddress, MaxLength(150), Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20), Display(Name = "Immatriculation")]
    public string Plate { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Société / Entité")]
    public string Company { get; set; } = string.Empty;

    [Required, Display(Name = "Date d'édition")]
    public DateTime IssueDate { get; set; }

    [Required, Display(Name = "Date de fin de validité")]
    public DateTime EndDate { get; set; }

    [MaxLength(150), Display(Name = "Délivré par")]
    public string IssuedBy { get; set; } = string.Empty;

    [MaxLength(1000), Display(Name = "Observations")]
    public string Notes { get; set; } = string.Empty;

    public bool Revoked { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp] public byte[]? RowVersion { get; set; }

    public string FullName => string.Join(" ", new[] { LastName, FirstName }.Where(s => !string.IsNullOrWhiteSpace(s)));
}
