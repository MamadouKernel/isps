using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.Entities;

public class NotificationSettings
{
    public int Id { get; set; }

    [MaxLength(200), Display(Name = "Serveur SMTP")]
    public string SmtpHost { get; set; } = string.Empty;

    [Range(1, 65535), Display(Name = "Port")]
    public int SmtpPort { get; set; } = 587;

    [MaxLength(200), Display(Name = "Utilisateur SMTP")]
    public string SmtpUsername { get; set; } = string.Empty;

    [MaxLength(200), Display(Name = "Mot de passe SMTP")]
    public string SmtpPassword { get; set; } = string.Empty;

    [Display(Name = "Utiliser STARTTLS")]
    public bool UseStartTls { get; set; } = true;

    [MaxLength(200), Display(Name = "Adresse expéditeur")]
    public string FromAddress { get; set; } = "no-reply@cit.ci";

    [MaxLength(200), Display(Name = "Nom expéditeur")]
    public string FromName { get; set; } = "Sûreté ISPS - CIT";

    [MaxLength(2000), Display(Name = "Destinataires (séparés par ; ou virgule)")]
    public string Recipients { get; set; } = string.Empty;

    [Display(Name = "Envoyer alertes J-30")]
    public bool EnableD30Alerts { get; set; } = true;

    [Display(Name = "Envoyer alertes J-7")]
    public bool EnableD7Alerts { get; set; } = true;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp] public byte[]? RowVersion { get; set; }
}
