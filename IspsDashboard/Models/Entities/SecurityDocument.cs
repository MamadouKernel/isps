using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

/// <summary>
/// Document de sûreté versionné (PFSP, procédures, consignes…) avec suivi de révision.
/// </summary>
public class SecurityDocument : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(200), Display(Name = "Titre")]
    public string Title { get; set; } = string.Empty;

    public DocumentCategory Category { get; set; } = DocumentCategory.Procedure;
    public DocumentStatus Status { get; set; } = DocumentStatus.Brouillon;

    [Required, MaxLength(20), Display(Name = "Version")]
    public string Version { get; set; } = "1.0";

    [MaxLength(150), Display(Name = "Auteur / Responsable")]
    public string Owner { get; set; } = string.Empty;

    [MaxLength(1000), Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Date de mise en vigueur")]
    public DateTime? EffectiveDate { get; set; }

    [Display(Name = "Prochaine révision")]
    public DateTime? NextReviewDate { get; set; }

    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(200)]
    public string FileName { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    [Display(Name = "Confidentiel")]
    public bool IsConfidential { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp] public byte[]? RowVersion { get; set; }
}
