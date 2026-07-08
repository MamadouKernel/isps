using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Implementations;
using Xunit;

namespace IspsDashboard.Tests;

/// <summary>
/// Vérifie que les actions "Create" reconstruisent une entité neuve côté serveur plutôt que
/// de persister l'objet lié depuis le formulaire tel quel — sans ça, un champ de formulaire
/// supplémentaire pourrait forger un statut/workflow qui n'a jamais réellement eu lieu.
/// Régression pour l'audit du 08/07/2026 (assignation de masse trouvée sur 10 services).
/// </summary>
public class MassAssignmentSecurityTests
{
    private static ApplicationDbContext Db() => TestHelpers.CreateInMemoryDb();

    [Fact]
    public async Task AuditCreate_ShouldIgnoreAttackerSuppliedStatusAndCompletedDate()
    {
        var db = Db();
        var service = new AuditCampaignService(db, new TestHelpers.NoOpAuditService());

        // Un attaquant ajoute des champs de formulaire non prévus par l'écran de création.
        var forged = new SecurityAudit
        {
            Title = "Audit trimestriel",
            ScheduledDate = DateTime.UtcNow.Date,
            Status = AuditStatus.Cloture,
            CompletedDate = DateTime.UtcNow,
            IsDeleted = true,
            Findings = { new AuditFinding { CheckItem = "Forgé", Result = FindingResult.Conforme } }
        };

        var saved = await service.CreateAsync(forged, seedChecklist: false);

        Assert.Equal(AuditStatus.Planifie, saved.Status);
        Assert.Null(saved.CompletedDate);
        Assert.False(saved.IsDeleted);
        Assert.Empty(saved.Findings);
    }

    [Fact]
    public async Task VisitorCreate_ShouldIgnoreAttackerSuppliedCheckInAndIsDeleted()
    {
        var db = Db();
        var service = new VisitorService(db, new TestHelpers.FixedClock(DateTime.UtcNow), new TestHelpers.NoOpAuditService());

        var forged = new Visitor
        {
            FullName = "Visiteur Test",
            Purpose = "Livraison",
            ScheduledArrival = DateTime.UtcNow,
            Status = VisitorStatus.SurSite,
            CheckInAt = DateTime.UtcNow,
            CheckedInBy = "attaquant",
            IsDeleted = true
        };

        var saved = await service.CreateAsync(forged);

        Assert.Equal(VisitorStatus.PreEnregistre, saved.Status);
        Assert.Null(saved.CheckInAt);
        Assert.Equal(string.Empty, saved.CheckedInBy);
        Assert.False(saved.IsDeleted);
    }

    [Fact]
    public async Task AccessPassCreate_ShouldIgnoreAttackerSuppliedRevokedAndIsDeleted()
    {
        var db = Db();
        var service = new AccessPassService(db, new TestHelpers.FixedClock(DateTime.UtcNow), new TestHelpers.NoOpAuditService());

        var forged = new AccessPass
        {
            LastName = "Test",
            IssueDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Revoked = true,
            IsDeleted = true
        };

        var saved = await service.CreateAsync(forged);

        Assert.False(saved.Revoked);
        Assert.False(saved.IsDeleted);
    }

    [Fact]
    public async Task NonConformityCreate_ShouldIgnoreAttackerSuppliedClosedAt()
    {
        var db = Db();
        var service = new NonConformityService(db, new TestHelpers.NoOpAuditService());

        var forged = new NonConformity
        {
            Title = "NC test",
            Description = "Description",
            IdentifiedAt = DateTime.UtcNow,
            ClosedAt = DateTime.UtcNow,
            ClosureEvidence = "Preuve forgée",
            Status = NonConformityStatus.Levee,
            IsDeleted = true
        };

        var saved = await service.CreateAsync(forged);

        Assert.Equal(NonConformityStatus.Identifiee, saved.Status);
        Assert.Null(saved.ClosedAt);
        Assert.Equal(string.Empty, saved.ClosureEvidence);
        Assert.False(saved.IsDeleted);
    }
}
