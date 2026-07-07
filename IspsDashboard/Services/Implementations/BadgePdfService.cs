using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace IspsDashboard.Services.Implementations;

public sealed class BadgePdfService : IBadgePdfService
{
    private readonly IQrCodeService _qr;

    public BadgePdfService(IQrCodeService qr) => _qr = qr;

    public byte[] BuildCheckpointSheet(IReadOnlyList<Checkpoint> checkpoints, string scanBaseUrl)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(t => t.FontSize(10).FontFamily(Fonts.Calibri));

                page.Header().Column(c =>
                {
                    c.Item().Text("Checkpoints de patrouille — QR à scanner").FontSize(15).Bold().FontColor("#0a1628");
                    c.Item().Text("À imprimer, plastifier et apposer à chaque point de contrôle.").FontSize(9).FontColor("#64748b");
                    c.Item().PaddingTop(4).LineHorizontal(2).LineColor("#d4a017");
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                    foreach (var cp in checkpoints)
                    {
                        var url = $"{scanBaseUrl.TrimEnd('/')}/Patrols/Scan/{cp.Code}";
                        var png = _qr.GeneratePng(url, 8);
                        table.Cell().Padding(5).Border(1).BorderColor("#cbd5e1").Padding(10).Column(col =>
                        {
                            col.Item().AlignCenter().Text(cp.Code).FontSize(16).Bold().FontColor("#0a1628");
                            col.Item().AlignCenter().Text(cp.Label).FontSize(10);
                            col.Item().AlignCenter().Text(cp.Zone).FontSize(8).FontColor("#64748b");
                            col.Item().AlignCenter().PaddingTop(6).Width(140).Image(png);
                            col.Item().AlignCenter().Text($"Fréquence cible : {cp.TargetIntervalMinutes} min").FontSize(8).FontColor("#64748b");
                        });
                    }
                });

                page.Footer().AlignCenter().Text("CDC-ISPS-2026-001 · Côte d'Ivoire Terminal").FontSize(8).FontColor("#94a3b8");
            });
        });
        return doc.GeneratePdf();
    }

    public byte[] BuildAccessPassBadge(AccessPass pass, string terminalTitle)
    {
        var typeLabel = pass.Type switch
        {
            AccessPassType.Journalier => "JOURNALIER",
            AccessPassType.Trimestriel => "TRIMESTRIEL",
            _ => "ANNUEL"
        };
        var qrContent = $"ISPS-CIT|{pass.Reference}|{pass.FullName}|{pass.EndDate:yyyy-MM-dd}";
        var qrPng = _qr.GeneratePng(qrContent, 8);

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                // Format badge ~ carte (85x54 mm) agrandi sur A6 pour impression
                page.Size(PageSizes.A6.Landscape());
                page.Margin(12);
                page.DefaultTextStyle(t => t.FontSize(9).FontFamily(Fonts.Calibri));
                page.PageColor("#0a1628");

                page.Content().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(terminalTitle).FontSize(11).Bold().FontColor("#ffffff");
                            c.Item().Text("Laissez-passer sûreté — ISPS").FontSize(7).FontColor("#22d3ee");
                            c.Item().PaddingTop(2).Width(90).LineHorizontal(1.5f).LineColor("#d4a017");
                        });
                        row.ConstantItem(70).Background("#22d3ee").Padding(4).AlignMiddle()
                            .Text(typeLabel).FontSize(9).Bold().FontColor("#0a1628").AlignCenter();
                    });

                    col.Item().PaddingTop(8).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(pass.FullName).FontSize(14).Bold().FontColor("#ffffff");
                            c.Item().Text(pass.Category == AccessPassCategory.Vehicule ? "🚚 Véhicule" : "👤 Personne").FontSize(8).FontColor("#cbd5e1");
                            if (!string.IsNullOrWhiteSpace(pass.Company))
                                c.Item().Text(pass.Company).FontSize(8).FontColor("#94a3b8");
                            c.Item().PaddingTop(4).Text($"Réf : {pass.Reference}").FontSize(8).FontColor("#22d3ee");
                            if (!string.IsNullOrWhiteSpace(pass.Matricule))
                                c.Item().Text($"Matricule : {pass.Matricule}").FontSize(8).FontColor("#cbd5e1");
                            if (!string.IsNullOrWhiteSpace(pass.Plate))
                                c.Item().Text($"Immat. : {pass.Plate}").FontSize(8).FontColor("#cbd5e1");
                            c.Item().Text($"Validité : {pass.IssueDate:dd/MM/yyyy} → {pass.EndDate:dd/MM/yyyy}").FontSize(8).FontColor("#cbd5e1");
                        });
                        row.ConstantItem(90).AlignRight().AlignMiddle().Background("#ffffff").Padding(4).Width(82).Image(qrPng);
                    });
                });
            });
        });
        return doc.GeneratePdf();
    }
}
