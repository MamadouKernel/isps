using ClosedXML.Excel;
using IspsDashboard.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace IspsDashboard.Services.Implementations;

public sealed class ExportService : IExportService
{
    public byte[] ToExcel(TableData data)
    {
        using var workbook = new XLWorkbook();
        var sheetName = Sanitize(data.Title);
        var ws = workbook.Worksheets.Add(sheetName);

        // Titre
        ws.Cell(1, 1).Value = data.Title;
        var titleRange = ws.Range(1, 1, 1, Math.Max(1, data.Headers.Count));
        titleRange.Merge();
        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 14;
        titleRange.Style.Font.FontColor = XLColor.FromHtml("#0A1628");

        // En-têtes
        const int headerRow = 3;
        for (int c = 0; c < data.Headers.Count; c++)
        {
            var cell = ws.Cell(headerRow, c + 1);
            cell.Value = data.Headers[c];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0A1628");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        }

        // Données
        for (int r = 0; r < data.Rows.Count; r++)
        {
            var row = data.Rows[r];
            for (int c = 0; c < row.Count; c++)
                ws.Cell(headerRow + 1 + r, c + 1).Value = row[c];
        }

        var used = ws.Range(headerRow, 1, headerRow + data.Rows.Count, Math.Max(1, data.Headers.Count));
        used.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        used.Style.Border.BottomBorderColor = XLColor.FromHtml("#CCCCCC");
        ws.Columns().AdjustToContents();
        ws.SheetView.FreezeRows(headerRow);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public byte[] ToPdf(TableData data)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(t => t.FontSize(9).FontFamily(Fonts.Calibri));

                page.Header().Column(c =>
                {
                    c.Item().Text(data.Title).FontSize(14).Bold().FontColor("#0a1628");
                    c.Item().Text($"Généré le {DateTime.Now:dd/MM/yyyy HH:mm} — {data.Rows.Count} ligne(s)")
                        .FontSize(8).FontColor("#64748b");
                    c.Item().PaddingTop(4).LineHorizontal(2).LineColor("#d4a017");
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        foreach (var _ in data.Headers) cols.RelativeColumn();
                    });

                    foreach (var h in data.Headers)
                        table.Cell().Background("#0a1628").Padding(4).Text(h).FontColor("#ffffff").FontSize(8).Bold();

                    foreach (var row in data.Rows)
                        foreach (var val in row)
                            table.Cell().BorderBottom(0.5f).BorderColor("#e2e8f0").Padding(4).Text(val ?? string.Empty).FontSize(8);
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("CDC-ISPS-2026-001 · Côte d'Ivoire Terminal — document confidentiel · page ").FontSize(8).FontColor("#94a3b8");
                    t.CurrentPageNumber().FontSize(8).FontColor("#94a3b8");
                    t.Span(" / ").FontSize(8).FontColor("#94a3b8");
                    t.TotalPages().FontSize(8).FontColor("#94a3b8");
                });
            });
        });
        return doc.GeneratePdf();
    }

    private static string Sanitize(string name)
    {
        var invalid = new[] { '\\', '/', '*', '?', ':', '[', ']' };
        var clean = new string(name.Where(ch => !invalid.Contains(ch)).ToArray());
        return clean.Length > 31 ? clean[..31] : clean;
    }
}
