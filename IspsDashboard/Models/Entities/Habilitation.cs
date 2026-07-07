using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Habilitation ou formation détenue par un agent, avec dates d'obtention et d'expiration.
/// </summary>
public class Habilitation : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    public int AgentId { get; set; }
    public Agent? Agent { get; set; }

    public HabilitationCategory Category { get; set; }

    [Required, MaxLength(150), Display(Name = "Intitulé")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Organisme délivreur")]
    public string Issuer { get; set; } = string.Empty;

    [Required, Display(Name = "Date d'obtention")]
    public DateTime ObtainedAt { get; set; }

    [Required, Display(Name = "Date d'expiration")]
    public DateTime ExpiresAt { get; set; }

    [MaxLength(80), Display(Name = "Référence / N°")]
    public string Reference { get; set; } = string.Empty;

    [MaxLength(500), Display(Name = "Justificatif (chemin)")]
    public string DocumentPath { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp] public byte[]? RowVersion { get; set; }
}
