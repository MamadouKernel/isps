using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.ViewModels;

public class IncidentSearchCriteria
{
    public IncidentStatus? Status { get; set; }
    public IncidentSeverity? Severity { get; set; }
    public IncidentCategory? Category { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Query { get; set; }
}

public class IncidentListViewModel
{
    public IReadOnlyList<Incident> Incidents { get; init; } = Array.Empty<Incident>();
    public IncidentSearchCriteria Criteria { get; init; } = new();
    public int TotalOpen { get; init; }
    public int TotalInvestigation { get; init; }
    public int TotalClosed { get; init; }
}

public class IncidentEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le titre est requis"), MaxLength(200)]
    [Display(Name = "Titre")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Catégorie")]
    public IncidentCategory Category { get; set; } = IncidentCategory.Autre;

    [Display(Name = "Sévérité")]
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Mineur;

    [Display(Name = "Statut")]
    public IncidentStatus Status { get; set; } = IncidentStatus.Ouvert;

    [Required(ErrorMessage = "La date est requise")]
    [Display(Name = "Date et heure de survenue")]
    public DateTime OccurredAt { get; set; } = DateTime.Now;

    [MaxLength(150), Display(Name = "Zone")]
    public string Zone { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Déclaré par")]
    public string ReportedBy { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Enquêteur")]
    public string Investigator { get; set; } = string.Empty;

    [Required(ErrorMessage = "La description est requise"), MaxLength(4000)]
    [Display(Name = "Description détaillée")]
    public string Description { get; set; } = string.Empty;

    [MaxLength(4000), Display(Name = "Mesures prises")]
    public string ActionsTaken { get; set; } = string.Empty;

    [MaxLength(4000), Display(Name = "Enseignements / REX")]
    public string LessonsLearned { get; set; } = string.Empty;

    public byte[]? RowVersion { get; set; }

    public List<IFormFile> NewAttachments { get; set; } = new();
}
