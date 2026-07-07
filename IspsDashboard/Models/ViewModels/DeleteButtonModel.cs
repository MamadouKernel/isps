namespace IspsDashboard.Models.ViewModels;

public sealed class DeleteButtonModel
{
    public required string Controller { get; set; }
    public string Action { get; set; } = "Delete";
    public Dictionary<string, string> RouteValues { get; set; } = new();
    public string ConfirmMessage { get; set; } = "Confirmer la suppression ?";
    public string ButtonLabel { get; set; } = "🗑️ Supprimer";
    public string ButtonClass { get; set; } = "btn-danger text-sm";
    public string FormClass { get; set; } = "inline";
}
