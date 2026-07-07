using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

public class NotificationRecord
{
    public int Id { get; set; }

    public NotificationKind Kind { get; set; }

    [Required, MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Recipients { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public bool DeliverySuccess { get; set; }

    public int? RelatedExerciseId { get; set; }
}
