using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.Entities;

public class KpiHistory
{
    public int Id { get; set; }

    public int KpiTableRowId { get; set; }
    public KpiTableRow? KpiTableRow { get; set; }

    [Required, MaxLength(50)]
    public string Value { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Period { get; set; } = string.Empty;

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
