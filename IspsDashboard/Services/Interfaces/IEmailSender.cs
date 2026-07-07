namespace IspsDashboard.Services.Interfaces;

public interface IEmailSender
{
    Task<EmailResult> SendAsync(IEnumerable<string> recipients, string subject, string htmlBody);
    Task<IReadOnlyList<string>> GetConfiguredRecipientsAsync();
}

public record EmailResult(bool Success, string? ErrorMessage = null);
