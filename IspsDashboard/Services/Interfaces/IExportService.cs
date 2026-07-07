namespace IspsDashboard.Services.Interfaces;

/// <summary>
/// Données tabulaires à exporter (titre, en-têtes, lignes).
/// </summary>
public sealed record TableData(string Title, IReadOnlyList<string> Headers, IReadOnlyList<IReadOnlyList<string>> Rows);

public interface IExportService
{
    byte[] ToExcel(TableData data);
    byte[] ToPdf(TableData data);
}
