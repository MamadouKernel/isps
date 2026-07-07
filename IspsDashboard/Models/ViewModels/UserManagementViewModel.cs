using System.ComponentModel.DataAnnotations;

namespace IspsDashboard.Models.ViewModels;

public class UserListItem
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateUserViewModel
{
    [Required, EmailAddress, Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required, Display(Name = "Nom complet")]
    public string FullName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(8), Display(Name = "Mot de passe")]
    public string Password { get; set; } = string.Empty;

    [Required, Display(Name = "Rôle")]
    public string Role { get; set; } = "Lecteur";
}

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [Required, Display(Name = "Nom complet")]
    public string FullName { get; set; } = string.Empty;

    [Required, Display(Name = "Rôle")]
    public string Role { get; set; } = "Lecteur";

    public DateTime CreatedAt { get; set; }

    [DataType(DataType.Password), MinLength(8), Display(Name = "Nouveau mot de passe (laisser vide pour ne pas changer)")]
    public string? NewPassword { get; set; }
}
