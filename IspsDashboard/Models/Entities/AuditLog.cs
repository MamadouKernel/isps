using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.Entities;

public class AuditLog
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    [MaxLength(150)]
    public string UserName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Detail { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
