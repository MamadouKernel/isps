using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

public enum ExerciseType
{
    Training = 0,
    GrandNature = 1,
    Crisis = 2
}

public class Exercise : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    public ExerciseType Type { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Icon { get; set; } = string.Empty;

    public DateTime PlannedDate { get; set; }

    [MaxLength(120)]
    public string Responsible { get; set; } = string.Empty;

    public ExerciseStatus Status { get; set; } = ExerciseStatus.Planifie;

    [MaxLength(2000)]
    public string Observations { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    [Timestamp] public byte[]? RowVersion { get; set; }
}
