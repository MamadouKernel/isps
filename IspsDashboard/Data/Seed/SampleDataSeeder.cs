using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Data.Seed;

/// <summary>
/// Données d'exemple à but de démonstration. Activé via la configuration "Seed:SampleData=true".
/// À désactiver en production pour démarrer avec une base vierge.
/// </summary>
public static class SampleDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, IConfiguration config)
    {
        var enabled = config.GetValue("Seed:SampleData", false);
        if (!enabled) return;

        var today = DateTime.UtcNow.Date;

        await SeedAccessPassesAsync(db, today);
        await SeedVehicleMovementsAsync(db, today);
        await SeedVisitorsAsync(db, today);
        await SeedIncidentsAsync(db, today);
        await SeedPfspDocumentAsync(db, today);

        await db.SaveChangesAsync();
    }

    private static async Task SeedAccessPassesAsync(ApplicationDbContext db, DateTime today)
    {
        if (await db.AccessPasses.AnyAsync()) return;

        int n = 1;
        string Ref() => $"LP-{today.Year}-{n++:D4}";

        db.AccessPasses.AddRange(
            new AccessPass { Reference = Ref(), Type = AccessPassType.Annuel, Category = AccessPassCategory.Personne,
                LastName = "Koné", FirstName = "Ibrahim", Contact = "+225 0701020304", Matricule = "AG-001",
                Email = "i.kone@cit.ci", Company = "Côte d'Ivoire Terminal", IssueDate = today.AddMonths(-2), EndDate = today.AddMonths(10), IssuedBy = "PFSO" },
            new AccessPass { Reference = Ref(), Type = AccessPassType.Annuel, Category = AccessPassCategory.Personne,
                LastName = "Aka", FirstName = "Mariam", Contact = "+225 0705060708", Matricule = "AG-002",
                Company = "Côte d'Ivoire Terminal", IssueDate = today.AddMonths(-1), EndDate = today.AddMonths(11), IssuedBy = "PFSO" },
            new AccessPass { Reference = Ref(), Type = AccessPassType.Trimestriel, Category = AccessPassCategory.Vehicule,
                LastName = "Traoré", FirstName = "Sékou", Contact = "+225 0709080706", Matricule = "TR-118",
                Plate = "AB-4521-CI", Company = "Transit Abidjan SA", IssueDate = today.AddDays(-10), EndDate = today.AddMonths(3).AddDays(-10), IssuedBy = "PFSO" },
            new AccessPass { Reference = Ref(), Type = AccessPassType.Trimestriel, Category = AccessPassCategory.Vehicule,
                LastName = "Bamba", FirstName = "Yaya", Contact = "+225 0700112233", Matricule = "TR-204",
                Plate = "CD-7788-CI", Company = "Maersk Logistics", IssueDate = today.AddDays(-5), EndDate = today.AddMonths(3).AddDays(-5), IssuedBy = "PFSO" },
            new AccessPass { Reference = Ref(), Type = AccessPassType.Journalier, Category = AccessPassCategory.Personne,
                LastName = "Yao", FirstName = "Christelle", Contact = "+225 0744556677", Matricule = "VIS-J-09",
                Company = "Bureau Veritas", IssueDate = today, EndDate = today.AddDays(1), IssuedBy = "PFSO" },
            new AccessPass { Reference = Ref(), Type = AccessPassType.Journalier, Category = AccessPassCategory.Vehicule,
                LastName = "Diomandé", FirstName = "Adama", Matricule = "TR-555",
                Plate = "EF-3344-CI", Company = "Bolloré Transport", IssueDate = today.AddDays(-3), EndDate = today.AddDays(-2), IssuedBy = "PFSO" }
        );
    }

    private static async Task SeedVehicleMovementsAsync(ApplicationDbContext db, DateTime today)
    {
        if (await db.VehicleAccesses.AnyAsync()) return;

        int n = 1;
        string Ref() => $"ACC-{today.Year}-{n++:D4}";

        db.VehicleAccesses.AddRange(
            new VehicleAccess { Reference = Ref(), Plate = "AB-4521-CI", Type = VehicleType.Camion, Direction = AccessDirection.Entree,
                Result = AccessControlResult.Autorise, DriverName = "Traoré Sékou", Carrier = "Transit Abidjan SA",
                ContainerNumber = "MSCU7654321", SealNumber = "SL-220199", SealVerified = true, BookingReference = "BK-99213",
                Searched = true, Controller = "Agent 03", Gate = "Portail principal PC6", OccurredAt = today.AddHours(8).AddMinutes(15) },
            new VehicleAccess { Reference = Ref(), Plate = "AB-4521-CI", Type = VehicleType.Camion, Direction = AccessDirection.Sortie,
                Result = AccessControlResult.Autorise, DriverName = "Traoré Sékou", Carrier = "Transit Abidjan SA",
                ContainerNumber = "MSCU7654321", SealNumber = "SL-220199", SealVerified = true,
                Controller = "Agent 03", Gate = "Portail principal PC6", OccurredAt = today.AddHours(9).AddMinutes(40) },
            new VehicleAccess { Reference = Ref(), Plate = "CD-7788-CI", Type = VehicleType.Conteneur, Direction = AccessDirection.Entree,
                Result = AccessControlResult.Autorise, DriverName = "Bamba Yaya", Carrier = "Maersk Logistics",
                ContainerNumber = "MAEU1122334", SealNumber = "SL-330871", SealVerified = true, BookingReference = "BK-99245",
                Searched = false, Controller = "Agent 05", Gate = "Portail nord camions", OccurredAt = today.AddHours(10).AddMinutes(5) },
            new VehicleAccess { Reference = Ref(), Plate = "ZZ-0000-XX", Type = VehicleType.Camion, Direction = AccessDirection.Entree,
                Result = AccessControlResult.Refuse, DriverName = "Inconnu", Carrier = "Non identifié",
                Searched = false, Controller = "Agent 05", Gate = "Portail nord camions", OccurredAt = today.AddHours(11),
                Notes = "Refus — pas de booking valide ni laissez-passer." }
        );
    }

    private static async Task SeedVisitorsAsync(ApplicationDbContext db, DateTime today)
    {
        if (await db.Visitors.AnyAsync()) return;

        int n = 1;
        string Ref() => $"VIS-{today.Year}-{n++:D4}";

        db.Visitors.AddRange(
            new Visitor { Reference = Ref(), FullName = "Christelle Yao", Company = "Bureau Veritas", IdDocumentNumber = "CI-008812",
                Phone = "+225 0744556677", Purpose = "Inspection certification", Host = "RSO Terminal", EscortedBy = "Agent 02",
                ScheduledArrival = today.AddHours(9), Status = VisitorStatus.SurSite, CheckInAt = DateTime.UtcNow.AddHours(-2), CheckedInBy = "admin@cit.ci", BadgeIssued = "V-014" },
            new Visitor { Reference = Ref(), FullName = "Jean-Marc Kouadio", Company = "Direction Générale", IdDocumentNumber = "CI-114520",
                Phone = "+225 0701239876", Purpose = "Réunion comité sûreté", Host = "Resp. Adjoint Sûreté",
                ScheduledArrival = today.AddHours(14), Status = VisitorStatus.PreEnregistre }
        );
    }

    private static async Task SeedIncidentsAsync(ApplicationDbContext db, DateTime today)
    {
        if (await db.Incidents.AnyAsync()) return;

        int n = 1;
        string Ref() => $"INC-{today.Year}-{n++:D4}";

        db.Incidents.AddRange(
            new Incident { Reference = Ref(), Title = "Tentative d'intrusion périmètre est", Category = IncidentCategory.IntrusionPerimetre,
                Severity = IncidentSeverity.Majeur, Status = IncidentStatus.Clos, OccurredAt = today.AddDays(-45).AddHours(2),
                Zone = "Périmètre est segment 2", ReportedBy = "Agent 07", Investigator = "PFSO",
                Description = "Détection mouvement par CCTV-004 à 02h12. Patrouille dépêchée, individu en fuite avant interception.",
                ActionsTaken = "Renforcement éclairage zone, ronde supplémentaire ajoutée la nuit.",
                LessonsLearned = "Angle mort partiel corrigé par réorientation de CCTV-004.", ClosedAt = today.AddDays(-43) },
            new Incident { Reference = Ref(), Title = "Colis abandonné quai Q4", Category = IncidentCategory.ColisSuspect,
                Severity = IncidentSeverity.Modere, Status = IncidentStatus.Clos, OccurredAt = today.AddDays(-18).AddHours(5),
                Zone = "Quai Q4", ReportedBy = "Agent 04", Investigator = "RSO",
                Description = "Sac non identifié signalé près du poste Q4. Périmètre sécurisé.",
                ActionsTaken = "Vérification — effets personnels d'un docker. Restitué.", ClosedAt = today.AddDays(-18) },
            new Incident { Reference = Ref(), Title = "Badge non restitué en fin de visite", Category = IncidentCategory.AnomalieBadge,
                Severity = IncidentSeverity.Mineur, Status = IncidentStatus.EnInvestigation, OccurredAt = today.AddDays(-2).AddHours(6),
                Zone = "Portail principal PC6", ReportedBy = "Agent 02",
                Description = "Visiteur sorti sans rendre le badge V-009. Relance en cours." }
        );
    }

    private static async Task SeedPfspDocumentAsync(ApplicationDbContext db, DateTime today)
    {
        if (await db.SecurityDocuments.AnyAsync()) return;

        db.SecurityDocuments.AddRange(
            new SecurityDocument { Title = "Plan de Sûreté de l'Installation Portuaire (PFSP)", Category = DocumentCategory.Pfsp,
                Status = DocumentStatus.EnVigueur, Version = "3.2", Owner = "PFSO", IsConfidential = true,
                Description = "Document maître de sûreté — approuvé par l'autorité compétente.",
                EffectiveDate = today.AddMonths(-8), NextReviewDate = today.AddMonths(4) },
            new SecurityDocument { Title = "Procédure de contrôle d'accès véhicules", Category = DocumentCategory.Procedure,
                Status = DocumentStatus.EnVigueur, Version = "1.4", Owner = "Resp. Adjoint Sûreté",
                Description = "Modalités de contrôle des camions et conteneurs aux portails.",
                EffectiveDate = today.AddMonths(-5), NextReviewDate = today.AddMonths(7) },
            new SecurityDocument { Title = "Consigne — montée au niveau MARSEC 2", Category = DocumentCategory.Consigne,
                Status = DocumentStatus.EnVigueur, Version = "2.0", Owner = "PFSO",
                Description = "Actions à appliquer en cas de passage au niveau de sûreté 2.",
                EffectiveDate = today.AddMonths(-3), NextReviewDate = today.AddMonths(9) }
        );
    }
}
