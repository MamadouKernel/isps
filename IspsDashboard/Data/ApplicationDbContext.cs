using System.Linq.Expressions;
using IspsDashboard.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DashboardSettings> DashboardSettings => Set<DashboardSettings>();
    public DbSet<KpiCard> KpiCards => Set<KpiCard>();
    public DbSet<KpiTableRow> KpiTableRows => Set<KpiTableRow>();
    public DbSet<Gauge> Gauges => Set<Gauge>();
    public DbSet<ProgressBar> ProgressBars => Set<ProgressBar>();
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<NotificationRecord> Notifications => Set<NotificationRecord>();
    public DbSet<NotificationSettings> NotificationSettings => Set<NotificationSettings>();
    public DbSet<KpiHistory> KpiHistories => Set<KpiHistory>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<IncidentAttachment> IncidentAttachments => Set<IncidentAttachment>();
    public DbSet<Habilitation> Habilitations => Set<Habilitation>();
    public DbSet<Visitor> Visitors => Set<Visitor>();
    public DbSet<Checkpoint> Checkpoints => Set<Checkpoint>();
    public DbSet<PatrolScan> PatrolScans => Set<PatrolScan>();
    public DbSet<MarsecLevelChange> MarsecLevelChanges => Set<MarsecLevelChange>();
    public DbSet<MarsecChecklistItem> MarsecChecklistItems => Set<MarsecChecklistItem>();
    public DbSet<NonConformity> NonConformities => Set<NonConformity>();
    public DbSet<VesselCall> VesselCalls => Set<VesselCall>();
    public DbSet<DeclarationOfSecurity> DeclarationsOfSecurity => Set<DeclarationOfSecurity>();
    public DbSet<Camera> Cameras => Set<Camera>();
    public DbSet<CameraMaintenance> CameraMaintenances => Set<CameraMaintenance>();
    public DbSet<ExternalContact> ExternalContacts => Set<ExternalContact>();
    public DbSet<ContactInteraction> ContactInteractions => Set<ContactInteraction>();
    public DbSet<ShiftBriefing> ShiftBriefings => Set<ShiftBriefing>();
    public DbSet<ExerciseRex> ExerciseRexes => Set<ExerciseRex>();
    public DbSet<VehicleAccess> VehicleAccesses => Set<VehicleAccess>();
    public DbSet<SecurityDocument> SecurityDocuments => Set<SecurityDocument>();
    public DbSet<SecurityAudit> SecurityAudits => Set<SecurityAudit>();
    public DbSet<AuditFinding> AuditFindings => Set<AuditFinding>();
    public DbSet<AccessPass> AccessPasses => Set<AccessPass>();
    public DbSet<RestrictedZone> RestrictedZones => Set<RestrictedZone>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<KpiHistory>()
            .HasOne(h => h.KpiTableRow)
            .WithMany()
            .HasForeignKey(h => h.KpiTableRowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Exercise>().HasIndex(e => new { e.Type, e.DisplayOrder });
        builder.Entity<KpiCard>().HasIndex(k => k.DisplayOrder);
        builder.Entity<KpiTableRow>().HasIndex(k => new { k.Category, k.DisplayOrder });
        builder.Entity<Agent>().HasIndex(a => a.Position).IsUnique();

        builder.Entity<Incident>().HasIndex(i => i.Reference).IsUnique();
        builder.Entity<Incident>().HasIndex(i => i.Status);
        builder.Entity<Incident>().HasIndex(i => i.OccurredAt);
        builder.Entity<IncidentAttachment>()
            .HasOne(a => a.Incident)
            .WithMany(i => i.Attachments)
            .HasForeignKey(a => a.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Habilitation>()
            .HasOne(h => h.Agent)
            .WithMany(a => a.Habilitations)
            .HasForeignKey(h => h.AgentId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Habilitation>().HasIndex(h => h.ExpiresAt);

        builder.Entity<Visitor>().HasIndex(v => v.Reference).IsUnique();
        builder.Entity<Visitor>().HasIndex(v => v.Status);
        builder.Entity<Visitor>().HasIndex(v => v.ScheduledArrival);

        builder.Entity<Checkpoint>().HasIndex(c => c.Code).IsUnique();
        builder.Entity<PatrolScan>().HasIndex(p => new { p.CheckpointId, p.ScannedAt });
        builder.Entity<PatrolScan>()
            .HasOne(p => p.Checkpoint)
            .WithMany()
            .HasForeignKey(p => p.CheckpointId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<PatrolScan>()
            .HasOne(p => p.Agent)
            .WithMany()
            .HasForeignKey(p => p.AgentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<MarsecChecklistItem>()
            .HasOne(i => i.MarsecLevelChange)
            .WithMany(c => c.ChecklistItems)
            .HasForeignKey(i => i.MarsecLevelChangeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<NonConformity>().HasIndex(n => n.Reference).IsUnique();
        builder.Entity<NonConformity>().HasIndex(n => n.Status);
        builder.Entity<NonConformity>().HasIndex(n => n.DueDate);

        builder.Entity<VesselCall>().HasIndex(v => v.Reference).IsUnique();
        builder.Entity<VesselCall>().HasIndex(v => v.Eta);
        builder.Entity<VesselCall>().HasIndex(v => v.Status);
        builder.Entity<DeclarationOfSecurity>().HasIndex(d => d.Reference).IsUnique();
        builder.Entity<DeclarationOfSecurity>()
            .HasOne(d => d.VesselCall)
            .WithMany(v => v.Declarations)
            .HasForeignKey(d => d.VesselCallId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Camera>().HasIndex(c => c.Code).IsUnique();
        builder.Entity<Camera>().HasIndex(c => c.Status);
        builder.Entity<CameraMaintenance>()
            .HasOne(m => m.Camera)
            .WithMany(c => c.MaintenanceHistory)
            .HasForeignKey(m => m.CameraId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ExternalContact>().HasIndex(c => c.Type);
        builder.Entity<ContactInteraction>()
            .HasOne(i => i.Contact)
            .WithMany(c => c.Interactions)
            .HasForeignKey(i => i.ExternalContactId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ShiftBriefing>().HasIndex(b => new { b.ShiftDate, b.Slot });
        builder.Entity<ExerciseRex>()
            .HasOne(r => r.Exercise)
            .WithMany()
            .HasForeignKey(r => r.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ExerciseRex>().HasIndex(r => r.ExerciseId).IsUnique();

        builder.Entity<VehicleAccess>().HasIndex(v => v.Reference).IsUnique();
        builder.Entity<VehicleAccess>().HasIndex(v => v.OccurredAt);
        builder.Entity<VehicleAccess>().HasIndex(v => v.Plate);

        builder.Entity<SecurityDocument>().HasIndex(d => d.Category);
        builder.Entity<SecurityDocument>().HasIndex(d => d.Status);

        builder.Entity<SecurityAudit>().HasIndex(a => a.Reference).IsUnique();
        builder.Entity<SecurityAudit>().HasIndex(a => a.Status);
        builder.Entity<AuditFinding>()
            .HasOne(f => f.Audit)
            .WithMany(a => a.Findings)
            .HasForeignKey(f => f.SecurityAuditId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<AccessPass>().HasIndex(p => p.Reference).IsUnique();
        builder.Entity<AccessPass>().HasIndex(p => p.Type);
        builder.Entity<AccessPass>().HasIndex(p => p.EndDate);

        builder.Entity<RestrictedZone>().HasIndex(z => z.Code).IsUnique();
        builder.Entity<RestrictedZone>().HasIndex(z => z.AccessLevel);

        // Filtre global : les entités soft-deleted sont automatiquement exclues de toutes les requêtes.
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var param = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(param, nameof(ISoftDeletable.IsDeleted));
                var filter = Expression.Lambda(Expression.Not(prop), param);
                builder.Entity(entityType.ClrType).HasQueryFilter(filter);
                builder.Entity(entityType.ClrType).HasIndex(nameof(ISoftDeletable.IsDeleted));
            }
        }
    }
}
