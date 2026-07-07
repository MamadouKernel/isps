namespace IspsDashboard.Services.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string detail, string? userId = null, string? userName = null);
}
