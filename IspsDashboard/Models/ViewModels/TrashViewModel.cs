namespace IspsDashboard.Models.ViewModels;

public sealed record TrashEntry(string TypeName, string TypeLabel, int Id, string Label, DateTime? DeletedAt, string? DeletedBy);

public class TrashViewModel
{
    public IReadOnlyList<TrashEntry> Entries { get; init; } = Array.Empty<TrashEntry>();
}
