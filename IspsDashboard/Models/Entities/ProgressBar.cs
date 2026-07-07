using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.Entities;

public class ProgressBar : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Label { get; set; } = string.Empty;

    [Range(0, 100)]
    public int Value { get; set; }

    public int DisplayOrder { get; set; }
}
