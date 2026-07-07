namespace IspsDashboard.Services.Interfaces;

public interface IPdfReportService
{
    /// <summary>Génère le rapport mensuel sûreté pour la période demandée.</summary>
    Task<byte[]> GenerateMonthlyReportAsync(int year, int month);
}
