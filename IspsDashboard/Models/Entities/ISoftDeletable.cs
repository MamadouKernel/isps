namespace IspsDashboard.Models.Entities;

/// <summary>
/// Entité supportant la suppression logique (soft delete) : l'enregistrement
/// n'est jamais physiquement effacé — il est masqué et reste restaurable/auditable.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedById { get; set; }
}
