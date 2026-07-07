using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace IspsDashboard.Services.Implementations;

public class SmtpEmailSender : IEmailSender
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;
    private readonly ISecretProtector _protector;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(
        ApplicationDbContext db,
        IConfiguration config,
        ISecretProtector protector,
        ILogger<SmtpEmailSender> logger)
    {
        _db = db;
        _config = config;
        _protector = protector;
        _logger = logger;
    }

    public async Task<EmailResult> SendAsync(IEnumerable<string> recipients, string subject, string htmlBody)
    {
        var (host, port, user, pwd, useTls, from, fromName) = await ResolveConfigAsync();
        var to = recipients.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList();

        if (string.IsNullOrWhiteSpace(host))
        {
            _logger.LogWarning("SMTP non configuré — email simulé: {Subject} → {Recipients}", subject, string.Join(",", to));
            return new EmailResult(false, "SMTP non configuré");
        }
        if (to.Count == 0)
            return new EmailResult(false, "Aucun destinataire");

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, from));
            foreach (var addr in to)
                message.To.Add(MailboxAddress.Parse(addr));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            var secureOption = useTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
            await client.ConnectAsync(host, port, secureOption);
            if (!string.IsNullOrEmpty(user))
                await client.AuthenticateAsync(user, pwd);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return new EmailResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Échec envoi email");
            return new EmailResult(false, ex.Message);
        }
    }

    public async Task<IReadOnlyList<string>> GetConfiguredRecipientsAsync()
    {
        var settings = await _db.NotificationSettings.AsNoTracking().FirstOrDefaultAsync();
        var raw = settings?.Recipients ?? string.Empty;
        return raw.Split(new[] { ';', ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                  .Where(s => s.Contains('@'))
                  .Distinct(StringComparer.OrdinalIgnoreCase)
                  .ToList();
    }

    private async Task<(string host, int port, string user, string pwd, bool useTls, string from, string fromName)> ResolveConfigAsync()
    {
        var db = await _db.NotificationSettings.AsNoTracking().FirstOrDefaultAsync();
        var section = _config.GetSection("Smtp");

        string Pick(string? db, string key) => !string.IsNullOrWhiteSpace(db) ? db : section[key] ?? string.Empty;

        var host = Pick(db?.SmtpHost, "Host");
        var portStr = !string.IsNullOrWhiteSpace(db?.SmtpHost) ? db!.SmtpPort.ToString() : section["Port"];
        int.TryParse(portStr, out var port);
        if (port == 0) port = 587;
        var user = Pick(db?.SmtpUsername, "Username");
        var storedPwd = Pick(db?.SmtpPassword, "Password");
        var pwd = _protector.Unprotect(storedPwd);
        var useTls = db?.UseStartTls ?? true;
        var from = Pick(db?.FromAddress, "From");
        if (string.IsNullOrEmpty(from)) from = "no-reply@cit.ci";
        var fromName = db?.FromName ?? "Sûreté ISPS - CIT";
        return (host, port, user, pwd, useTls, from, fromName);
    }
}
