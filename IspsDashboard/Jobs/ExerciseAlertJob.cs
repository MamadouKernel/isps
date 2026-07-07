using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace IspsDashboard.Jobs;

[DisallowConcurrentExecution]
public class ExerciseAlertJob : IJob
{
    private readonly ApplicationDbContext _db;
    private readonly IExerciseService _exerciseService;
    private readonly IEmailSender _emailSender;
    private readonly IRazorTemplateRenderer _renderer;
    private readonly IClock _clock;
    private readonly ILogger<ExerciseAlertJob> _logger;

    public ExerciseAlertJob(
        ApplicationDbContext db,
        IExerciseService exerciseService,
        IEmailSender emailSender,
        IRazorTemplateRenderer renderer,
        IClock clock,
        ILogger<ExerciseAlertJob> logger)
    {
        _db = db;
        _exerciseService = exerciseService;
        _emailSender = emailSender;
        _renderer = renderer;
        _clock = clock;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var upcoming = await _exerciseService.GetUpcomingAsync(30);
        if (upcoming.Count == 0) return;

        var settings = await _db.NotificationSettings.AsNoTracking().FirstOrDefaultAsync();
        var recipients = await _emailSender.GetConfiguredRecipientsAsync();
        if (recipients.Count == 0)
        {
            _logger.LogWarning("Aucun destinataire configuré pour les alertes — exécution ignorée");
            return;
        }

        var terminal = (await _db.DashboardSettings.AsNoTracking().FirstOrDefaultAsync())?.TerminalTitle
                       ?? "Côte d'Ivoire Terminal";

        foreach (var exercise in upcoming)
        {
            var days = _exerciseService.ComputeDaysRemaining(exercise);
            var kind = ResolveKind(days, settings);
            if (kind is null) continue;

            var alreadySent = await _db.Notifications.AnyAsync(n =>
                n.RelatedExerciseId == exercise.Id &&
                n.Kind == kind.Value &&
                n.SentAt.Date == _clock.Today);
            if (alreadySent) continue;

            var subject = $"[Sûreté ISPS] Exercice « {exercise.Name} » dans {days} jour(s)";
            var body = await _renderer.RenderAsync("/Views/EmailTemplates/ExerciseAlert.cshtml",
                new ExerciseAlertEmailModel { Exercise = exercise, DaysRemaining = days, TerminalTitle = terminal });

            var result = await _emailSender.SendAsync(recipients, subject, body);
            _db.Notifications.Add(new NotificationRecord
            {
                Kind = kind.Value,
                Message = subject,
                Recipients = string.Join(";", recipients),
                DeliverySuccess = result.Success,
                RelatedExerciseId = exercise.Id,
                SentAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            _logger.LogInformation("Alerte exercice {Name} (J-{Days}): {Status}",
                exercise.Name, days, result.Success ? "envoyée" : $"échec — {result.ErrorMessage}");
        }
    }

    private static NotificationKind? ResolveKind(int days, NotificationSettings? settings) => days switch
    {
        <= 7 and >= 0 when settings?.EnableD7Alerts ?? true => NotificationKind.ExerciseUpcoming7,
        30 or 29 when settings?.EnableD30Alerts ?? true => NotificationKind.ExerciseUpcoming30,
        _ => null
    };
}
