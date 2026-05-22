using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using GradingTool.Application.Common.Interfaces;
using System.Text;

namespace GradingTool.Infrastructure.Services;

public class DocxTextExtractor : IDocxTextExtractor
{
    public Task<string> ExtractTextAsync(string docxFilePath)
    {
        using var doc = WordprocessingDocument.Open(docxFilePath, isEditable: false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body == null) return Task.FromResult(string.Empty);

        var sb = new StringBuilder();
        foreach (var para in body.Descendants<Paragraph>())
        {
            sb.AppendLine(para.InnerText);
        }

        return Task.FromResult(sb.ToString().TrimEnd());
    }
}
