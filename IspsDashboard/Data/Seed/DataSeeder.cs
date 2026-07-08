using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Data.Seed;

public static class DataSeeder
{
    public const string AdminRole = "Administrateur";
    public const string EditorRole = "Editeur";
    public const string ReaderRole = "Lecteur";

    public static async Task SeedAsync(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        // Les migrations sont écrites pour SQL Server (production/dev) ; les tests d'intégration
        // tournent sur SQLite en mémoire, incompatible avec le rejeu de ces migrations telles
        // quelles — on y crée le schéma directement depuis le modèle courant à la place.
        if (db.Database.IsSqlServer())
            await db.Database.MigrateAsync();
        else
            await db.Database.EnsureCreatedAsync();

        // Toujours : structure minimale indispensable au fonctionnement du compte admin.
        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager, configuration);

        // Données de référence (paramètres, KPI, agents, caméras, contacts, checkpoints, zones…)
        // Désactivé par défaut → en production la base démarre vide, hormis rôles + compte admin.
        // Activer via "Seed:ReferenceData": true (ex. en développement).
        if (configuration.GetValue("Seed:ReferenceData", false))
            await SeedDashboardAsync(db);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in new[] { AdminRole, EditorRole, ReaderRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var email = configuration["Seed:AdminEmail"] ?? "admin@cit.ci";
        var password = configuration["Seed:AdminPassword"];
        var fullName = configuration["Seed:AdminFullName"] ?? "Responsable Adjoint Sûreté";

        if (await userManager.FindByEmailAsync(email) is null)
        {
            // Pas de mot de passe par défaut en dur dans le code : il doit venir de la config
            // (User Secrets en développement, variable d'environnement en production).
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Aucun compte admin n'existe et « Seed:AdminPassword » n'est pas configuré. " +
                    "Définissez-le via `dotnet user-secrets set Seed:AdminPassword \"...\"` en développement, " +
                    "ou la variable d'environnement Seed__AdminPassword en production.");
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName
            };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, AdminRole);
            }
            else
            {
                // Échec bloquant : sans compte admin, l'application est inutilisable.
                // (Cause fréquente : mot de passe ne respectant pas la politique —
                //  min 8 caractères, 1 majuscule, 1 chiffre, 1 caractère spécial.)
                var errors = string.Join(" ; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(
                    $"Création du compte administrateur « {email} » impossible : {errors}");
            }
        }
    }

    private static async Task SeedDashboardAsync(ApplicationDbContext db)
    {
        if (!await db.DashboardSettings.AnyAsync())
        {
            db.DashboardSettings.Add(new DashboardSettings
            {
                TerminalTitle = "Côte d'Ivoire Terminal",
                Period = "Juin 2026",
                IspsLevel = 1,
                ResponsibleName = "Responsable Adjoint Sûreté",
                AgentsRequired = 12
            });
        }

        if (!await db.NotificationSettings.AnyAsync())
        {
            db.NotificationSettings.Add(new NotificationSettings
            {
                SmtpHost = string.Empty,
                SmtpPort = 587,
                UseStartTls = true,
                FromAddress = "no-reply@cit.ci",
                FromName = "Sûreté ISPS - CIT",
                EnableD30Alerts = true,
                EnableD7Alerts = true
            });
        }

        if (!await db.KpiCards.AnyAsync())
        {
            db.KpiCards.AddRange(
                new KpiCard { Label = "Disponibilité CCTV", Value = "100", Subtitle = "% caméras opérationnelles", TrendBadge = "= stable", Color = KpiCardColor.Vert, DisplayOrder = 1 },
                new KpiCard { Label = "Agents en poste", Value = "11/12", Subtitle = "Effectif requis 12", TrendBadge = "▼ 1 absent", Color = KpiCardColor.Ambre, DisplayOrder = 2 },
                new KpiCard { Label = "Réaction alarme", Value = "3.8 min", Subtitle = "Seuil ≤ 5 min", TrendBadge = "▼ -0.4", Color = KpiCardColor.Vert, DisplayOrder = 3 },
                new KpiCard { Label = "Alarmes injustifiées", Value = "4", Subtitle = "Seuil ≤ 5 / mois", TrendBadge = "▲ +1", Color = KpiCardColor.Ambre, DisplayOrder = 4 },
                new KpiCard { Label = "Incidents sûreté", Value = "0", Subtitle = "Objectif 0 / mois", TrendBadge = "= stable", Color = KpiCardColor.Vert, DisplayOrder = 5 },
                new KpiCard { Label = "Accès non autorisés", Value = "0", Subtitle = "Objectif 0 / mois", TrendBadge = "= stable", Color = KpiCardColor.Vert, DisplayOrder = 6 }
            );
        }

        if (!await db.Gauges.AnyAsync())
        {
            db.Gauges.AddRange(
                new Gauge { Label = "Personnel formé ISPS", Value = 92, DisplayOrder = 1 },
                new Gauge { Label = "REX rédigés", Value = 100, DisplayOrder = 2 },
                new Gauge { Label = "Non-conformités levées", Value = 87, DisplayOrder = 3 },
                new Gauge { Label = "Participation aux exercices", Value = 95, DisplayOrder = 4 }
            );
        }

        if (!await db.ProgressBars.AnyAsync())
        {
            db.ProgressBars.AddRange(
                new ProgressBar { Label = "Signalements anonymes traités", Value = 97, DisplayOrder = 1 },
                new ProgressBar { Label = "Audit ISPS dans les 12 derniers mois", Value = 100, DisplayOrder = 2 },
                new ProgressBar { Label = "Habilitations valides", Value = 100, DisplayOrder = 3 },
                new ProgressBar { Label = "Badges vérifiés aux entrées", Value = 98, DisplayOrder = 4 },
                new ProgressBar { Label = "Plan PFSP à jour", Value = 100, DisplayOrder = 5 }
            );
        }

        if (!await db.Agents.AnyAsync())
        {
            for (int i = 1; i <= 12; i++)
            {
                db.Agents.Add(new Agent
                {
                    Name = $"Agent {i:00}",
                    IsPresent = i != 7,
                    Position = i
                });
            }
        }

        if (!await db.Exercises.AnyAsync())
        {
            var today = DateTime.UtcNow.Date;
            db.Exercises.AddRange(
                new Exercise { Type = ExerciseType.Training, Name = "Intrusion périmétrique", Icon = "🛡️", PlannedDate = today.AddDays(45), Responsible = "Chef d'équipe sûreté", Status = ExerciseStatus.Planifie, DisplayOrder = 1 },
                new Exercise { Type = ExerciseType.Training, Name = "Colis suspect / Engin explosif", Icon = "📦", PlannedDate = today.AddDays(20), Responsible = "RSO Terminal", Status = ExerciseStatus.Planifie, DisplayOrder = 2 },
                new Exercise { Type = ExerciseType.Training, Name = "Contrôle d'accès renforcé", Icon = "🚪", PlannedDate = today.AddDays(5), Responsible = "Resp. Adjoint Sûreté", Status = ExerciseStatus.Planifie, DisplayOrder = 3 },
                new Exercise { Type = ExerciseType.Training, Name = "Montée en niveau ISPS", Icon = "🚨", PlannedDate = today.AddDays(90), Responsible = "Direction Sûreté", Status = ExerciseStatus.Planifie, DisplayOrder = 4 },

                new Exercise { Type = ExerciseType.GrandNature, Name = "Exercice Grandeur Nature annuel", Icon = "🎯", PlannedDate = today.AddDays(150), Responsible = "Direction + Autorités Portuaires", Status = ExerciseStatus.Planifie, Observations = "Simulation complète multi-scénarios coordonnée avec secours externes", DisplayOrder = 1 },

                new Exercise { Type = ExerciseType.Crisis, Name = "Activation cellule de crise", Icon = "🆘", PlannedDate = today.AddDays(60), Responsible = "Direction Générale", Status = ExerciseStatus.Planifie, DisplayOrder = 1 },
                new Exercise { Type = ExerciseType.Crisis, Name = "Communication de crise", Icon = "📢", PlannedDate = today.AddDays(25), Responsible = "Direction Communication", Status = ExerciseStatus.Planifie, DisplayOrder = 2 },
                new Exercise { Type = ExerciseType.Crisis, Name = "Confinement / Évacuation", Icon = "🚷", PlannedDate = today.AddDays(80), Responsible = "HSE", Status = ExerciseStatus.Planifie, DisplayOrder = 3 },
                new Exercise { Type = ExerciseType.Crisis, Name = "Coordination secours externes", Icon = "🚑", PlannedDate = today.AddDays(110), Responsible = "Resp. Adjoint Sûreté", Status = ExerciseStatus.Planifie, DisplayOrder = 4 }
            );
        }

        if (!await db.KpiTableRows.AnyAsync())
        {
            db.KpiTableRows.AddRange(
                new KpiTableRow { Category = "Sûreté Physique", Indicator = "Disponibilité caméras CCTV", CurrentValue = "100%", Target = "100%", Threshold = "< 98%", Status = KpiStatus.Conforme, DisplayOrder = 1 },
                new KpiTableRow { Category = "Sûreté Physique", Indicator = "Alarmes injustifiées/mois", CurrentValue = "4", Target = "≤ 5", Threshold = "≥ 6", Status = KpiStatus.Attention, DisplayOrder = 2 },
                new KpiTableRow { Category = "Sûreté Physique", Indicator = "Temps réaction sur alarme", CurrentValue = "3.8 min", Target = "≤ 5 min", Threshold = "> 5 min", Status = KpiStatus.Conforme, DisplayOrder = 3 },
                new KpiTableRow { Category = "Sûreté Physique", Indicator = "Éclairage périmétrique", CurrentValue = "100%", Target = "100%", Threshold = "< 100%", Status = KpiStatus.Conforme, DisplayOrder = 4 },
                new KpiTableRow { Category = "Ressources Humaines", Indicator = "Agents en poste / requis", CurrentValue = "11/12", Target = "12/12", Threshold = "< 11", Status = KpiStatus.Attention, DisplayOrder = 5 },
                new KpiTableRow { Category = "Ressources Humaines", Indicator = "Personnel formé ISPS", CurrentValue = "92%", Target = "100%", Threshold = "< 80%", Status = KpiStatus.Attention, DisplayOrder = 6 },
                new KpiTableRow { Category = "Ressources Humaines", Indicator = "Habilitations valides", CurrentValue = "100%", Target = "100%", Threshold = "< 95%", Status = KpiStatus.Conforme, DisplayOrder = 7 },
                new KpiTableRow { Category = "Ressources Humaines", Indicator = "Taux absentéisme", CurrentValue = "8%", Target = "≤ 5%", Threshold = "> 10%", Status = KpiStatus.Attention, DisplayOrder = 8 },
                new KpiTableRow { Category = "Contrôle Accès", Indicator = "Badges vérifiés / entrées", CurrentValue = "98%", Target = "100%", Threshold = "< 95%", Status = KpiStatus.Attention, DisplayOrder = 9 },
                new KpiTableRow { Category = "Contrôle Accès", Indicator = "Accès non autorisés/mois", CurrentValue = "0", Target = "0", Threshold = "≥ 1", Status = KpiStatus.Conforme, DisplayOrder = 10 },
                new KpiTableRow { Category = "Contrôle Accès", Indicator = "Délai contrôle camion", CurrentValue = "2.5 min", Target = "≤ 3 min", Threshold = "> 3 min", Status = KpiStatus.Conforme, DisplayOrder = 11 },
                new KpiTableRow { Category = "Contrôle Accès", Indicator = "Visiteurs enregistrés", CurrentValue = "100%", Target = "100%", Threshold = "< 100%", Status = KpiStatus.Conforme, DisplayOrder = 12 },
                new KpiTableRow { Category = "Incidents", Indicator = "Incidents sûreté/mois", CurrentValue = "0", Target = "0", Threshold = "≥ 1", Status = KpiStatus.Conforme, DisplayOrder = 13 },
                new KpiTableRow { Category = "Incidents", Indicator = "Intrusions détectées/mois", CurrentValue = "0", Target = "0", Threshold = "≥ 1", Status = KpiStatus.Conforme, DisplayOrder = 14 },
                new KpiTableRow { Category = "Incidents", Indicator = "Délai transmission rapport", CurrentValue = "6 h", Target = "≤ 24 h", Threshold = "> 24 h", Status = KpiStatus.Conforme, DisplayOrder = 15 },
                new KpiTableRow { Category = "Conformité ISPS", Indicator = "Non-conformités levées", CurrentValue = "87%", Target = "100%", Threshold = "< 80%", Status = KpiStatus.Attention, DisplayOrder = 16 },
                new KpiTableRow { Category = "Conformité ISPS", Indicator = "Délai depuis dernier audit", CurrentValue = "8 mois", Target = "≤ 12 mois", Threshold = "> 11 mois", Status = KpiStatus.Conforme, DisplayOrder = 17 },
                new KpiTableRow { Category = "Conformité ISPS", Indicator = "Recommandations audit en attente", CurrentValue = "3", Target = "0", Threshold = "> 5", Status = KpiStatus.Attention, DisplayOrder = 18 },
                new KpiTableRow { Category = "Conformité ISPS", Indicator = "Plan PFSP à jour", CurrentValue = "Oui", Target = "Oui", Threshold = "Non", Status = KpiStatus.Conforme, DisplayOrder = 19 },
                new KpiTableRow { Category = "Formation", Indicator = "Sessions formation/trimestre", CurrentValue = "1", Target = "≥ 1", Threshold = "0", Status = KpiStatus.Conforme, DisplayOrder = 20 },
                new KpiTableRow { Category = "Formation", Indicator = "Campagnes sensibilisation/semestre", CurrentValue = "1", Target = "≥ 2", Threshold = "0", Status = KpiStatus.Attention, DisplayOrder = 21 },
                new KpiTableRow { Category = "Reporting", Indicator = "Signalements anonymes traités", CurrentValue = "97%", Target = "≥ 95%", Threshold = "< 90%", Status = KpiStatus.Conforme, DisplayOrder = 22 },
                new KpiTableRow { Category = "Reporting", Indicator = "REX rédigés", CurrentValue = "100%", Target = "100%", Threshold = "< 80%", Status = KpiStatus.Conforme, DisplayOrder = 23 }
            );
        }

        if (!await db.Cameras.AnyAsync())
        {
            db.Cameras.AddRange(
                new Camera { Code = "CCTV-001", Label = "Portail principal PC6", Zone = "Entrée", Type = Models.Enums.CameraType.PTZ, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-002", Label = "Portail nord (camions)", Zone = "Entrée", Type = Models.Enums.CameraType.LecteurPlaques, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-003", Label = "Périmètre est segment 1", Zone = "Périmètre", Type = Models.Enums.CameraType.Fixe, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-004", Label = "Périmètre est segment 2", Zone = "Périmètre", Type = Models.Enums.CameraType.Fixe, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-005", Label = "Périmètre ouest segment 1", Zone = "Périmètre", Type = Models.Enums.CameraType.Fixe, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-006", Label = "Périmètre ouest segment 2", Zone = "Périmètre", Type = Models.Enums.CameraType.Fixe, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-007", Label = "Quai Q4 nord", Zone = "Quai", Type = Models.Enums.CameraType.PTZ, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-008", Label = "Quai Q4 sud", Zone = "Quai", Type = Models.Enums.CameraType.PTZ, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-009", Label = "Zone reefer", Zone = "Stockage", Type = Models.Enums.CameraType.Dome, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-010", Label = "Stockage carburant", Zone = "Sensibles", Type = Models.Enums.CameraType.Thermique, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-011", Label = "Local CCTV", Zone = "Technique", Type = Models.Enums.CameraType.Fixe, Status = Models.Enums.CameraStatus.Operationnelle },
                new Camera { Code = "CCTV-012", Label = "Parc engins de manutention", Zone = "Stockage", Type = Models.Enums.CameraType.Dome, Status = Models.Enums.CameraStatus.Operationnelle }
            );
        }

        if (!await db.ExternalContacts.AnyAsync())
        {
            db.ExternalContacts.AddRange(
                new ExternalContact { Name = "Port Autonome d'Abidjan (PAA) — PSO", Type = Models.Enums.ContactType.AutoritePortuaire, Role = "PSO du PAA", PrimaryPhone = "+225 00 00 00 00", EmergencyPhone = "+225 00 00 00 01", IsEmergency24x7 = true },
                new ExternalContact { Name = "Gendarmerie maritime — Abidjan", Type = Models.Enums.ContactType.GendarmerieMaritime, Role = "Brigade portuaire", PrimaryPhone = "+225 00 00 00 00", EmergencyPhone = "+225 00 00 00 01", IsEmergency24x7 = true },
                new ExternalContact { Name = "Sapeurs-Pompiers Treichville", Type = Models.Enums.ContactType.Pompiers, Role = "Caserne PC6", PrimaryPhone = "18", EmergencyPhone = "18", IsEmergency24x7 = true },
                new ExternalContact { Name = "SAMU Côte d'Ivoire", Type = Models.Enums.ContactType.Samu, Role = "Urgences médicales", PrimaryPhone = "185", IsEmergency24x7 = true },
                new ExternalContact { Name = "Police nationale — Commissariat Treichville", Type = Models.Enums.ContactType.Police, Role = "Police", PrimaryPhone = "17", EmergencyPhone = "17", IsEmergency24x7 = true },
                new ExternalContact { Name = "Direction Générale des Douanes", Type = Models.Enums.ContactType.Douanes, Role = "Brigade portuaire", PrimaryPhone = "+225 00 00 00 00", IsEmergency24x7 = false }
            );
        }

        if (!await db.Checkpoints.AnyAsync())
        {
            db.Checkpoints.AddRange(
                new Checkpoint { Code = "CP-01", Label = "Portail principal PC6", Zone = "Entrée", TargetIntervalMinutes = 60 },
                new Checkpoint { Code = "CP-02", Label = "Portail nord camions", Zone = "Entrée", TargetIntervalMinutes = 60 },
                new Checkpoint { Code = "CP-03", Label = "Périmètre est (clôture)", Zone = "Périmètre", TargetIntervalMinutes = 180 },
                new Checkpoint { Code = "CP-04", Label = "Périmètre ouest (clôture)", Zone = "Périmètre", TargetIntervalMinutes = 180 },
                new Checkpoint { Code = "CP-05", Label = "Quai conteneurs Q4", Zone = "Quai", TargetIntervalMinutes = 240 },
                new Checkpoint { Code = "CP-06", Label = "Zone reefer", Zone = "Stockage", TargetIntervalMinutes = 240 },
                new Checkpoint { Code = "CP-07", Label = "Local technique CCTV", Zone = "Technique", TargetIntervalMinutes = 480 },
                new Checkpoint { Code = "CP-08", Label = "Stockage carburant", Zone = "Sensibles", TargetIntervalMinutes = 120 }
            );
        }

        if (!await db.RestrictedZones.AnyAsync())
        {
            db.RestrictedZones.AddRange(
                new RestrictedZone { Code = "ZAR-01", Name = "Local technique CCTV & serveurs sûreté", AccessLevel = Models.Enums.ZoneAccessLevel.Sensible,
                    Description = "Cœur du système de vidéosurveillance et d'alarme.", ZoneManager = "RSO Terminal",
                    ProtectionMeasures = "Porte blindée, badge + code, journal d'accès, CCTV interne.",
                    AuthorizedPersonnel = "RSO, PFSO, technicien CCTV habilité.", RequiresEscort = true, RequiresClearance = true, CctvMonitored = true },
                new RestrictedZone { Code = "ZAR-02", Name = "Stockage carburant", AccessLevel = Models.Enums.ZoneAccessLevel.Sensible,
                    Description = "Cuves et zone de dépotage carburant.", ZoneManager = "HSE",
                    ProtectionMeasures = "Clôture dédiée, détection incendie, interdiction de fumer, rondes renforcées.",
                    AuthorizedPersonnel = "Personnel HSE, agents de sûreté en ronde.", RequiresEscort = false, RequiresClearance = true, CctvMonitored = true },
                new RestrictedZone { Code = "ZAR-03", Name = "Quai conteneurs Q4 (zone opérationnelle)", AccessLevel = Models.Enums.ZoneAccessLevel.Restreint,
                    Description = "Zone d'opérations navire — interface ship/shore.", ZoneManager = "Resp. Exploitation",
                    ProtectionMeasures = "Contrôle d'accès gangway, badges, supervision CCTV PTZ.",
                    AuthorizedPersonnel = "Dockers habilités, agents sûreté, équipage en escale (avec DoS).", CctvMonitored = true },
                new RestrictedZone { Code = "ZAR-04", Name = "Poste de commandement sûreté (PC)", AccessLevel = Models.Enums.ZoneAccessLevel.Restreint,
                    Description = "Centre de pilotage de la sûreté du terminal.", ZoneManager = "Resp. Adjoint Sûreté",
                    ProtectionMeasures = "Accès badge nominatif, journal, redondance électrique.",
                    AuthorizedPersonnel = "Équipe sûreté, direction.", RequiresClearance = true, CctvMonitored = true },
                new RestrictedZone { Code = "ZAR-05", Name = "Zone reefer (conteneurs frigorifiques)", AccessLevel = Models.Enums.ZoneAccessLevel.Controle,
                    Description = "Parc de conteneurs frigorifiques sous tension.", ZoneManager = "Resp. Exploitation",
                    ProtectionMeasures = "Badge, signalisation, CCTV dôme.",
                    AuthorizedPersonnel = "Techniciens reefer, dockers.", CctvMonitored = true }
            );
        }

        await db.SaveChangesAsync();
    }
}
