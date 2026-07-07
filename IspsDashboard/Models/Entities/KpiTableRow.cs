using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

public class KpiTableRow : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Indicator { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string CurrentValue { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Target { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Threshold { get; set; } = string.Empty;

    public KpiStatus Status { get; set; } = KpiStatus.Conforme;

    public int DisplayOrder { get; set; }

    [Timestamp] public byte[]? RowVersion { get; set; }
}
