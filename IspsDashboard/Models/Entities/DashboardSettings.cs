using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.Entities;

public class DashboardSettings
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string TerminalTitle { get; set; } = "Côte d'Ivoire Terminal";

    [MaxLength(200)]
    public string Period { get; set; } = string.Empty;

    [Range(1, 3)]
    public int IspsLevel { get; set; } = 1;

    [MaxLength(150)]
    public string ResponsibleName { get; set; } = string.Empty;

    public int AgentsRequired { get; set; } = 12;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedById { get; set; }

    [Timestamp] public byte[]? RowVersion { get; set; }
}
