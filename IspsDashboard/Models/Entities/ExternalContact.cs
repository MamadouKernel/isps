using System.ComponentModel.DataAnnotations;
using IspsDashboard.Models.Enums;

namespace IspsDashboard.Models.Entities;

public class ExternalContact : ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }

    public int Id { get; set; }

    [Required, MaxLength(150), Display(Name = "Nom de l'organisme / personne")]
    public string Name { get; set; } = string.Empty;

    public ContactType Type { get; set; } = ContactType.Autre;

    [MaxLength(120), Display(Name = "Fonction / titre")]
    public string Role { get; set; } = string.Empty;

    [MaxLength(30), Display(Name = "Téléphone principal")]
    public string PrimaryPhone { get; set; } = string.Empty;

    [MaxLength(30), Display(Name = "Téléphone d'urgence")]
    public string EmergencyPhone { get; set; } = string.Empty;

    [EmailAddress, MaxLength(150), Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(300), Display(Name = "Adresse")]
    public string Address { get; set; } = string.Empty;

    [MaxLength(80), Display(Name = "Canal radio")]
    public string RadioChannel { get; set; } = string.Empty;

    [Display(Name = "Contact d'urgence (24/7)")]
    public bool IsEmergency24x7 { get; set; }

    [MaxLength(1000), Display(Name = "Notes")]
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ContactInteraction> Interactions { get; set; } = new();
}

public class ContactInteraction
{
    public int Id { get; set; }

    public int ExternalContactId { get; set; }
    public ExternalContact? Contact { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public InteractionDirection Direction { get; set; }
    public InteractionChannel Channel { get; set; }

    [Required, MaxLength(200), Display(Name = "Objet")]
    public string Subject { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Interlocuteur côté terminal")]
    public string HandledBy { get; set; } = string.Empty;

    [MaxLength(2000), Display(Name = "Compte-rendu")]
    public string Summary { get; set; } = string.Empty;
}
