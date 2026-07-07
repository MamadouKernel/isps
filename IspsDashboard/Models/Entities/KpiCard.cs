using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

public class KpiCard : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Label { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Value { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Subtitle { get; set; } = string.Empty;

    [MaxLength(50)]
    public string TrendBadge { get; set; } = string.Empty;

    public KpiCardColor Color { get; set; } = KpiCardColor.Vert;

    public int DisplayOrder { get; set; }

    [Timestamp] public byte[]? RowVersion { get; set; }
}
