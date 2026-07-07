using System.Globalization;
using IspsDashboard.Data;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace IspsDashboard.Services.Implementations;

public sealed class PdfReportService : IPdfReportService
{
    private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr-FR");

    static PdfReportService()
    {
        // Licence Community pour QuestPDF (gratuit < $1M revenu annuel).
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private readonly ApplicationDbContext _db;
    private readonly ICameraService _cameras;

    public PdfReportService(ApplicationDbContext db, ICameraService cameras)
    {
        _db = db;
        _cameras = cameras;
    }

    public async Task<byte[]> GenerateMonthlyReportAsync(int year, int month)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);

        var settings = await _db.DashboardSettings.AsNoTracking().FirstOrDefaultAsync();
        var incidents = await _db.Incidents.AsNoTracking()
            .Where(i => i.OccurredAt >= from && i.OccurredAt < to)
            .OrderBy(i => i.OccurredAt)
            .ToListAsync();
        var exercises = await _db.Exercises.AsNoTracking()
            .Where(e => e.PlannedDate >= from && e.PlannedDate < to)
            .OrderBy(e => e.PlannedDate)
            .ToListAsync();
        var nc = await _db.NonConformities.AsNoTracking()
            .Where(n => n.IdentifiedAt >= from && n.IdentifiedAt < to)
            .OrderBy(n => n.IdentifiedAt)
            .ToListAsync();
        var marsecChanges = await _db.MarsecLevelChanges.AsNoTracking()
            .Where(c => c.ChangedAt >= from && c.ChangedAt < to)
            .OrderBy(c => c.ChangedAt)
            .ToListAsync();
        var cameraAvailability = await _cameras.AvailabilityPercentAsync();
        var openIncidents = await _db.Incidents.CountAsync(i => i.Status != IncidentStatus.Clos);
        var openNc = await _db.NonConformities.CountAsync(n =>
            n.Status != NonConformityStatus.Levee && n.Status != NonConformityStatus.Rejetee);

        var monthLabel = from.ToString("MMMM yyyy", Fr);
        var terminal = settings?.TerminalTitle ?? "Côte d'Ivoire Terminal";
        var ispsLevel = settings?.IspsLevel ?? 1;

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(t => t.FontSize(10).FontFamily(Fonts.Calibri));
                page.PageColor(Colors.White);

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(terminal).FontSize(16).Bold().FontColor("#0a1628");
                            c.Item().Text("Rapport mensuel sûreté — Code ISPS").FontSize(10).FontColor("#475569");
                        });
                        row.ConstantItem(120).AlignRight().Column(c =>
                        {
                            c.Item().Text(monthLabel.ToUpperInvariant()).FontSize(12).Bold().FontColor("#0a1628");
                            c.Item().Text($"Niveau MARSEC : N°{ispsLevel}").FontSize(10);
                        });
                    });
                    col.Item().PaddingTop(4).LineHorizontal(2).LineColor("#d4a017");
                });

                page.Content().PaddingVertical(15).Column(content =>
                {
                    content.Spacing(15);

                    content.Item().Element(e => SectionTitle(e, "📊 Synthèse du mois"));
                    content.Item().Row(row =>
                    {
                        row.Spacing(8);
                        row.RelativeItem().Element(c => KpiCard(c, "Incidents du mois", incidents.Count.ToString(), "#0a1628"));
                        row.RelativeItem().Element(c => KpiCard(c, "Exercices réalisés",
                            exercises.Count(e => e.Status == ExerciseStatus.Realise).ToString(), "#10b981"));
                        row.RelativeItem().Element(c => KpiCard(c, "NC ouvertes (total)", openNc.ToString(), "#f59e0b"));
                        row.RelativeItem().Element(c => KpiCard(c, "Dispo. CCTV", $"{cameraAvailability}%", "#06b6d4"));
                        row.RelativeItem().Element(c => KpiCard(c, "Incidents ouverts", openIncidents.ToString(), "#ef4444"));
                    });

                    content.Item().Element(e => SectionTitle(e, "🚨 Incidents du mois"));
                    if (incidents.Count == 0)
                    {
                        content.Item().Text("Aucun incident enregistré sur la période.").Italic().FontColor("#64748b");
                    }
                    else
                    {
                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(70);
                                c.ConstantColumn(70);
                                c.RelativeColumn(2);
                                c.ConstantColumn(70);
                                c.ConstantColumn(70);
                            });
                            HeaderCell(table, "Réf.");
                            HeaderCell(table, "Date");
                            HeaderCell(table, "Titre");
                            HeaderCell(table, "Sévérité");
                            HeaderCell(table, "Statut");
                            foreach (var i in incidents)
                            {
                                BodyCell(table, i.Reference);
                                BodyCell(table, i.OccurredAt.ToString("dd/MM HH:mm"));
                                BodyCell(table, i.Title);
                                BodyCell(table, i.Severity.ToString());
                                BodyCell(table, i.Status.ToString());
                            }
                        });
                    }

                    content.Item().Element(e => SectionTitle(e, "🏋️ Exercices"));
                    if (exercises.Count == 0)
                    {
                        content.Item().Text("Aucun exercice planifié sur la période.").Italic().FontColor("#64748b");
                    }
                    else
                    {
                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.ConstantColumn(70);
                                c.RelativeColumn();
                                c.ConstantColumn(60);
                            });
                            HeaderCell(table, "Nom");
                            HeaderCell(table, "Date");
                            HeaderCell(table, "Responsable");
                            HeaderCell(table, "Statut");
                            foreach (var e in exercises)
                            {
                                BodyCell(table, e.Name);
                                BodyCell(table, e.PlannedDate.ToString("dd/MM"));
                                BodyCell(table, e.Responsible);
                                BodyCell(table, e.Status.ToString());
                            }
                        });
                    }

                    content.Item().Element(e => SectionTitle(e, "❗ Non-conformités identifiées"));
                    if (nc.Count == 0)
                    {
                        content.Item().Text("Aucune non-conformité enregistrée sur la période.").Italic().FontColor("#64748b");
                    }
                    else
                    {
                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(70);
                                c.RelativeColumn(2);
                                c.RelativeColumn();
                                c.ConstantColumn(60);
                            });
                            HeaderCell(table, "Réf.");
                            HeaderCell(table, "Intitulé");
                            HeaderCell(table, "Source");
                            HeaderCell(table, "Statut");
                            foreach (var n in nc)
                            {
                                BodyCell(table, n.Reference);
                                BodyCell(table, n.Title);
                                BodyCell(table, n.Source.ToString());
                                BodyCell(table, n.Status.ToString());
                            }
                        });
                    }

                    content.Item().Element(e => SectionTitle(e, "⚓ Changements de niveau MARSEC"));
                    if (marsecChanges.Count == 0)
                    {
                        content.Item().Text("Aucun changement de niveau sur la période.").Italic().FontColor("#64748b");
                    }
                    else
                    {
                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(90);
                                c.ConstantColumn(70);
                                c.RelativeColumn(2);
                                c.RelativeColumn();
                            });
                            HeaderCell(table, "Date");
                            HeaderCell(table, "Transition");
                            HeaderCell(table, "Motif");
                            HeaderCell(table, "Décidé par");
                            foreach (var m in marsecChanges)
                            {
                                BodyCell(table, m.ChangedAt.ToString("dd/MM HH:mm"));
                                BodyCell(table, $"N°{m.FromLevel} → N°{m.ToLevel}");
                                BodyCell(table, m.Reason);
                                BodyCell(table, m.DecidedBy);
                            }
                        });
                    }
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("CDC-ISPS-2026-001 · Document confidentiel — Côte d'Ivoire Terminal · page ").FontSize(8).FontColor("#94a3b8");
                    t.CurrentPageNumber().FontSize(8).FontColor("#94a3b8");
                    t.Span(" / ").FontSize(8).FontColor("#94a3b8");
                    t.TotalPages().FontSize(8).FontColor("#94a3b8");
                });
            });
        });

        return pdf.GeneratePdf();
    }

    private static void SectionTitle(IContainer container, string title)
        => container.PaddingTop(4).BorderBottom(1).BorderColor("#cbd5e1").PaddingBottom(2)
            .Text(title).FontSize(12).Bold().FontColor("#0a1628");

    private static void KpiCard(IContainer container, string label, string value, string accent)
        => container.Background("#f8fafc").Border(1).BorderColor("#e2e8f0").Padding(8)
            .Column(c =>
            {
                c.Item().Text(label).FontSize(8).FontColor("#64748b");
                c.Item().Text(value).FontSize(16).Bold().FontColor(accent);
            });

    private static void HeaderCell(QuestPDF.Fluent.TableDescriptor table, string text)
        => table.Cell().Background("#0a1628").Padding(4).Text(text).FontColor("#ffffff").FontSize(9).Bold();

    private static void BodyCell(QuestPDF.Fluent.TableDescriptor table, string text)
        => table.Cell().BorderBottom(0.5f).BorderColor("#e2e8f0").Padding(4).Text(text ?? string.Empty).FontSize(9);
}
