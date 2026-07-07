using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

public class Incident : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    /// <summary>Référence métier de type INC-2026-0001 générée à la création.</summary>
    [Required, MaxLength(20)]
    public string Reference { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public IncidentCategory Category { get; set; } = IncidentCategory.Autre;
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Mineur;
    public IncidentStatus Status { get; set; } = IncidentStatus.Ouvert;

    [Required]
    public DateTime OccurredAt { get; set; }

    [MaxLength(150)]
    public string Zone { get; set; } = string.Empty;

    [MaxLength(150)]
    public string ReportedBy { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Investigator { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string ActionsTaken { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string LessonsLearned { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }

    public string? CreatedById { get; set; }
    public string? ClosedById { get; set; }

    public List<IncidentAttachment> Attachments { get; set; } = new();

    [Timestamp] public byte[]? RowVersion { get; set; }
}

public class IncidentAttachment
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public Incident? Incident { get; set; }

    [Required, MaxLength(200)]
    public string FileName { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string StoredPath { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
