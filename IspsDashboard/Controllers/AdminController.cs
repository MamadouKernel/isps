using IspsDashboard.Data;
using IspsDashboard.Data.Seed;
using IspsDashboard.Hubs;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Controllers;

[Authorize(Policy = "RequireEditor")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IDashboardService _dashboardService;
    private readonly IAuditService _audit;
    private readonly IHubContext<DashboardHub> _hub;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _env;
    private readonly ISecretProtector _protector;

    public AdminController(
        ApplicationDbContext db,
        IDashboardService dashboardService,
        IAuditService audit,
        IHubContext<DashboardHub> hub,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IWebHostEnvironment env,
        ISecretProtector protector)
    {
        _db = db;
        _dashboardService = dashboardService;
        _audit = audit;
        _hub = hub;
        _userManager = userManager;
        _emailSender = emailSender;
        _env = env;
        _protector = protector;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _dashboardService.BuildAdminAsync();
        ViewBag.NotificationSettings = await _db.NotificationSettings.AsNoTracking().FirstOrDefaultAsync()
            ?? new NotificationSettings();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequestSizeLimit(2_000_000)]
    public async Task<IActionResult> UploadLogo(IFormFile? logoFile)
    {
        if (logoFile is null || logoFile.Length == 0)
        {
            TempData["Error"] = "Aucun fichier sélectionné.";
            return RedirectToAction(nameof(Index));
        }

        var allowed = new[] { ".svg", ".png", ".jpg", ".jpeg", ".webp" };
        var ext = Path.GetExtension(logoFile.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext))
        {
            TempData["Error"] = "Format non supporté. Acceptés : SVG, PNG, JPG, WEBP.";
            return RedirectToAction(nameof(Index));
        }

        var imagesDir = Path.Combine(_env.WebRootPath, "images");
        Directory.CreateDirectory(imagesDir);

        foreach (var ex in allowed)
        {
            var existing = Path.Combine(imagesDir, "logo" + ex);
            if (System.IO.File.Exists(existing)) System.IO.File.Delete(existing);
        }

        var destination = Path.Combine(imagesDir, "logo" + ext);
        using (var stream = System.IO.File.Create(destination))
            await logoFile.CopyToAsync(stream);

        await _audit.LogAsync("UploadLogo", $"{logoFile.FileName} ({logoFile.Length} octets)");
        TempData["Success"] = "Logo mis à jour.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveNotificationSettings([Bind(Prefix = "Notifications")] NotificationSettings input)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Configuration non enregistrée : vérifiez les champs (port entre 1 et 65535, longueurs maximales).";
            return RedirectToAction(nameof(Index));
        }

        var existing = await _db.NotificationSettings.FirstOrDefaultAsync();
        if (existing is null)
        {
            existing = new NotificationSettings();
            _db.NotificationSettings.Add(existing);
        }
        existing.SmtpHost = input.SmtpHost ?? string.Empty;
        existing.SmtpPort = input.SmtpPort > 0 ? input.SmtpPort : 587;
        existing.SmtpUsername = input.SmtpUsername ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(input.SmtpPassword))
            existing.SmtpPassword = _protector.Protect(input.SmtpPassword);
        existing.UseStartTls = input.UseStartTls;
        existing.FromAddress = input.FromAddress ?? "no-reply@cit.ci";
        existing.FromName = input.FromName ?? "Sûreté ISPS - CIT";
        existing.Recipients = input.Recipients ?? string.Empty;
        existing.EnableD30Alerts = input.EnableD30Alerts;
        existing.EnableD7Alerts = input.EnableD7Alerts;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateNotificationSettings",
            $"SMTP={existing.SmtpHost}:{existing.SmtpPort}, dest={existing.Recipients.Length} car., D30={existing.EnableD30Alerts}, D7={existing.EnableD7Alerts}");
        TempData["Success"] = "Configuration des notifications enregistrée.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SendTestEmail(string? testRecipient)
    {
        var recipients = string.IsNullOrWhiteSpace(testRecipient)
            ? await _emailSender.GetConfiguredRecipientsAsync()
            : new[] { testRecipient.Trim() };

        if (recipients.Count == 0)
        {
            TempData["Error"] = "Aucun destinataire (renseignez la liste ou saisissez une adresse de test).";
            return RedirectToAction(nameof(Index));
        }

        var subject = "[TEST] Sûreté ISPS — Côte d'Ivoire Terminal";
        var body = $@"<p>Ceci est un email de test envoyé depuis le tableau de bord ISPS.</p>
<p>Si vous recevez ce message, la configuration SMTP est opérationnelle.</p>
<p style='color:#94a3b8;font-size:12px'>Émis le {DateTime.Now:dd/MM/yyyy HH:mm} par {User.Identity?.Name}</p>";

        var result = await _emailSender.SendAsync(recipients, subject, body);
        await _audit.LogAsync("TestEmail",
            $"{string.Join(",", recipients)} → {(result.Success ? "OK" : "ÉCHEC: " + result.ErrorMessage)}");

        if (result.Success)
            TempData["Success"] = $"Email de test envoyé à {string.Join(", ", recipients)}.";
        else
            TempData["Error"] = $"Échec d'envoi : {result.ErrorMessage}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSettings([Bind(Prefix = "Settings")] DashboardSettings input)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Paramètres non enregistrés : vérifiez les champs (titre obligatoire, longueurs maximales).";
            return RedirectToAction(nameof(Index));
        }

        var existing = await _db.DashboardSettings.FirstOrDefaultAsync();
        if (existing is null)
        {
            existing = new DashboardSettings();
            _db.DashboardSettings.Add(existing);
        }
        existing.TerminalTitle = string.IsNullOrWhiteSpace(input.TerminalTitle) ? existing.TerminalTitle : input.TerminalTitle.Trim();
        existing.Period = input.Period?.Trim() ?? string.Empty;
        existing.IspsLevel = Math.Clamp(input.IspsLevel, 1, 3);
        existing.ResponsibleName = input.ResponsibleName?.Trim() ?? string.Empty;
        existing.AgentsRequired = input.AgentsRequired;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedById = _userManager.GetUserId(User);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateSettings", $"Niveau ISPS={existing.IspsLevel}, Période={existing.Period}");
        await NotifyDashboardChanged();
        TempData["Success"] = "Paramètres généraux enregistrés.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveKpiCards([FromForm] List<KpiCard> cards)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Cartes KPI non enregistrées : vérifiez les champs (libellé/valeur obligatoires, longueurs maximales).";
            return RedirectToAction(nameof(Index));
        }

        foreach (var input in cards)
        {
            var entity = await _db.KpiCards.FindAsync(input.Id);
            if (entity is null) continue;
            entity.Label = input.Label;
            entity.Value = input.Value;
            entity.Subtitle = input.Subtitle;
            entity.TrendBadge = input.TrendBadge;
            entity.Color = input.Color;
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateKpiCards", $"{cards.Count} cartes mises à jour");
        await NotifyDashboardChanged();
        TempData["Success"] = "Cartes KPI enregistrées.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddKpiCard(string label, string value, string? subtitle, string? trendBadge, KpiCardColor color)
    {
        var order = (await _db.KpiCards.MaxAsync(k => (int?)k.DisplayOrder) ?? 0) + 1;
        _db.KpiCards.Add(new KpiCard
        {
            Label = string.IsNullOrWhiteSpace(label) ? "Nouvel indicateur" : label.Trim(),
            Value = value?.Trim() ?? "0",
            Subtitle = subtitle?.Trim() ?? string.Empty,
            TrendBadge = trendBadge?.Trim() ?? string.Empty,
            Color = color,
            DisplayOrder = order
        });
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AddKpiCard", label ?? "(sans libellé)");
        await NotifyDashboardChanged();
        TempData["Success"] = "Carte KPI ajoutée.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteKpiCard(int id)
    {
        var entity = await _db.KpiCards.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedById = _userManager.GetUserId(User);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("DeleteKpiCard", entity.Label);
            await NotifyDashboardChanged();
        }
        TempData["Success"] = "Carte KPI supprimée (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveGauges([FromForm] List<Gauge> gauges)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Jauges non enregistrées : vérifiez les champs (libellé obligatoire, valeur entre 0 et 100).";
            return RedirectToAction(nameof(Index));
        }

        foreach (var input in gauges)
        {
            var entity = await _db.Gauges.FindAsync(input.Id);
            if (entity is null) continue;
            entity.Label = input.Label;
            entity.Value = Math.Clamp(input.Value, 0, 100);
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateGauges", $"{gauges.Count} jauges mises à jour");
        await NotifyDashboardChanged();
        TempData["Success"] = "Jauges enregistrées.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddGauge(string label, int value)
    {
        var order = (await _db.Gauges.MaxAsync(g => (int?)g.DisplayOrder) ?? 0) + 1;
        _db.Gauges.Add(new Gauge
        {
            Label = string.IsNullOrWhiteSpace(label) ? "Nouvelle jauge" : label.Trim(),
            Value = Math.Clamp(value, 0, 100),
            DisplayOrder = order
        });
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AddGauge", label ?? "(sans libellé)");
        await NotifyDashboardChanged();
        TempData["Success"] = "Jauge ajoutée.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteGauge(int id)
    {
        var entity = await _db.Gauges.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedById = _userManager.GetUserId(User);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("DeleteGauge", entity.Label);
            await NotifyDashboardChanged();
        }
        TempData["Success"] = "Jauge supprimée (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProgressBars([FromForm] List<ProgressBar> bars)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Barres non enregistrées : vérifiez les champs (libellé obligatoire, valeur entre 0 et 100).";
            return RedirectToAction(nameof(Index));
        }

        foreach (var input in bars)
        {
            var entity = await _db.ProgressBars.FindAsync(input.Id);
            if (entity is null) continue;
            entity.Label = input.Label;
            entity.Value = Math.Clamp(input.Value, 0, 100);
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateProgressBars", $"{bars.Count} barres mises à jour");
        await NotifyDashboardChanged();
        TempData["Success"] = "Barres de progression enregistrées.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProgressBar(string label, int value)
    {
        var order = (await _db.ProgressBars.MaxAsync(b => (int?)b.DisplayOrder) ?? 0) + 1;
        _db.ProgressBars.Add(new ProgressBar
        {
            Label = string.IsNullOrWhiteSpace(label) ? "Nouvelle barre" : label.Trim(),
            Value = Math.Clamp(value, 0, 100),
            DisplayOrder = order
        });
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AddProgressBar", label ?? "(sans libellé)");
        await NotifyDashboardChanged();
        TempData["Success"] = "Barre de progression ajoutée.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProgressBar(int id)
    {
        var entity = await _db.ProgressBars.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedById = _userManager.GetUserId(User);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("DeleteProgressBar", entity.Label);
            await NotifyDashboardChanged();
        }
        TempData["Success"] = "Barre de progression supprimée (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAgents([FromForm] List<Agent> agents)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Effectif non enregistré : vérifiez les champs (nom obligatoire, longueur maximale).";
            return RedirectToAction(nameof(Index));
        }

        foreach (var input in agents)
        {
            var entity = await _db.Agents.FindAsync(input.Id);
            if (entity is null) continue;
            entity.Name = input.Name;
            entity.IsPresent = input.IsPresent;
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateAgents", $"Présents: {agents.Count(a => a.IsPresent)}/{agents.Count}");
        await NotifyDashboardChanged();
        TempData["Success"] = "Effectif agents enregistré.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAgentPosition(string name)
    {
        var position = (await _db.Agents.MaxAsync(a => (int?)a.Position) ?? 0) + 1;
        _db.Agents.Add(new Agent
        {
            Name = string.IsNullOrWhiteSpace(name) ? $"Agent #{position}" : name.Trim(),
            Position = position,
            IsPresent = true
        });
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AddAgentPosition", $"Position #{position} — {name}");
        await NotifyDashboardChanged();
        TempData["Success"] = "Position ajoutée au tableau de garde.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAgentPosition(int id)
    {
        var entity = await _db.Agents.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedById = _userManager.GetUserId(User);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("DeleteAgentPosition", $"Position #{entity.Position} — {entity.Name}");
            await NotifyDashboardChanged();
        }
        TempData["Success"] = "Position retirée du tableau de garde (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveExercises([FromForm] List<Exercise> exercises)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Exercices non enregistrés : vérifiez les champs (nom obligatoire, longueurs maximales).";
            return RedirectToAction(nameof(Index));
        }

        foreach (var input in exercises)
        {
            var entity = await _db.Exercises.FindAsync(input.Id);
            if (entity is null) continue;
            entity.Name = input.Name;
            entity.PlannedDate = input.PlannedDate;
            entity.Responsible = input.Responsible;
            entity.Status = input.Status;
            entity.Observations = input.Observations ?? string.Empty;
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateExercises", $"{exercises.Count} exercices mis à jour");
        await NotifyDashboardChanged();
        TempData["Success"] = "Exercices enregistrés.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExercise(ExerciseType type, string name, DateTime plannedDate, string? responsible, ExerciseStatus status, string? icon)
    {
        if (type == ExerciseType.GrandNature && await _db.Exercises.AnyAsync(e => e.Type == ExerciseType.GrandNature))
        {
            TempData["Error"] = "Un exercice Grandeur Nature existe déjà. Supprimez-le avant d'en ajouter un nouveau.";
            return RedirectToAction(nameof(Index));
        }

        var order = (await _db.Exercises.Where(e => e.Type == type).MaxAsync(e => (int?)e.DisplayOrder) ?? 0) + 1;
        _db.Exercises.Add(new Exercise
        {
            Type = type,
            Name = string.IsNullOrWhiteSpace(name) ? "Nouvel exercice" : name.Trim(),
            Icon = icon?.Trim() ?? string.Empty,
            PlannedDate = plannedDate == default ? DateTime.UtcNow.Date : plannedDate,
            Responsible = responsible?.Trim() ?? string.Empty,
            Status = status,
            DisplayOrder = order
        });
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AddExercise", $"{type} / {name}");
        await NotifyDashboardChanged();
        TempData["Success"] = "Exercice ajouté.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExercise(int id)
    {
        var entity = await _db.Exercises.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedById = _userManager.GetUserId(User);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("DeleteExercise", entity.Name);
            await NotifyDashboardChanged();
        }
        TempData["Success"] = "Exercice supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveKpiTable([FromForm] List<KpiTableRow> rows)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Tableau KPIs non enregistré : vérifiez les champs (catégorie/indicateur obligatoires, longueurs maximales).";
            return RedirectToAction(nameof(Index));
        }

        foreach (var input in rows)
        {
            var entity = await _db.KpiTableRows.FindAsync(input.Id);
            if (entity is null) continue;
            entity.Category = input.Category;
            entity.Indicator = input.Indicator;
            entity.CurrentValue = input.CurrentValue;
            entity.Target = input.Target;
            entity.Threshold = input.Threshold;
            entity.Status = input.Status;

            _db.KpiHistories.Add(new KpiHistory
            {
                KpiTableRowId = entity.Id,
                Value = entity.CurrentValue,
                Period = DateTime.UtcNow.ToString("yyyy-MM")
            });
        }
        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdateKpiTable", $"{rows.Count} lignes du tableau mises à jour");
        await NotifyDashboardChanged();
        TempData["Success"] = "Tableau KPIs enregistré.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddKpiRow(string category, string indicator, string? currentValue, string? target, string? threshold, KpiStatus status)
    {
        var order = (await _db.KpiTableRows.MaxAsync(k => (int?)k.DisplayOrder) ?? 0) + 1;
        _db.KpiTableRows.Add(new KpiTableRow
        {
            Category = string.IsNullOrWhiteSpace(category) ? "Autre" : category.Trim(),
            Indicator = string.IsNullOrWhiteSpace(indicator) ? "Nouvel indicateur" : indicator.Trim(),
            CurrentValue = currentValue?.Trim() ?? string.Empty,
            Target = target?.Trim() ?? string.Empty,
            Threshold = threshold?.Trim() ?? string.Empty,
            Status = status,
            DisplayOrder = order
        });
        await _db.SaveChangesAsync();
        await _audit.LogAsync("AddKpiRow", $"{category} / {indicator}");
        await NotifyDashboardChanged();
        TempData["Success"] = "Indicateur ajouté au tableau.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteKpiRow(int id)
    {
        var entity = await _db.KpiTableRows.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedById = _userManager.GetUserId(User);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("DeleteKpiRow", entity.Indicator);
            await NotifyDashboardChanged();
        }
        TempData["Success"] = "Indicateur supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> AuditLogs()
    {
        var logs = await _db.AuditLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(200)
            .ToListAsync();
        return View(logs);
    }

    private async Task NotifyDashboardChanged()
    {
        _dashboardService.InvalidateCache();
        await _hub.Clients.All.SendAsync(DashboardHub.DashboardUpdatedMethod);
    }
}
