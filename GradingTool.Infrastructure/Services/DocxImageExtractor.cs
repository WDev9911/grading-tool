using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using GradingTool.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace GradingTool.Infrastructure.Services;

public class DocxImageExtractor : IDocxImageExtractor
{
    private readonly ILogger<DocxImageExtractor> _logger;

    public DocxImageExtractor(ILogger<DocxImageExtractor> logger)
    {
        _logger = logger;
    }

    public async Task<List<(byte[] Bytes, string Extension)>> ExtractImagesAsync(string docxFilePath)
    {
        var result = new List<(byte[] Bytes, string Extension)>();

        await Task.Run(() =>
        {
            using var doc = WordprocessingDocument.Open(docxFilePath, false);
            var mainPart = doc.MainDocumentPart;
            if (mainPart?.Document?.Body == null) return;

            // Collect relationship IDs in document order (first appearance)
            var seen = new HashSet<string>();
            var relIds = mainPart.Document.Body
                .Descendants<Blip>()
                .Select(b => b.Embed?.Value)
                .Where(id => !string.IsNullOrEmpty(id) && seen.Add(id!))
                .ToList();

            if (!relIds.Any())
            {
                // Fallback: just grab all image parts in undefined order
                foreach (var imagePart in mainPart.ImageParts)
                {
                    var ext = ContentTypeToExtension(imagePart.ContentType);
                    using var stream = imagePart.GetStream();
                    using var ms = new MemoryStream();
                    stream.CopyTo(ms);
                    result.Add((ms.ToArray(), ext));
                }
                return;
            }

            foreach (var relId in relIds)
            {
                try
                {
                    var part = mainPart.GetPartById(relId!);
                    if (part is not ImagePart imagePart) continue;

                    var ext = ContentTypeToExtension(imagePart.ContentType);
                    using var stream = imagePart.GetStream();
                    using var ms = new MemoryStream();
                    stream.CopyTo(ms);
                    result.Add((ms.ToArray(), ext));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not read image part {RelId}", relId);
                }
            }
        });

        return result;
    }

    private static string ContentTypeToExtension(string contentType) =>
        contentType.ToLowerInvariant() switch
        {
            "image/png"  => ".png",
            "image/jpeg" => ".jpg",
            "image/jpg"  => ".jpg",
            "image/gif"  => ".gif",
            "image/bmp"  => ".bmp",
            "image/tiff" => ".tiff",
            "image/webp" => ".webp",
            _            => ".img"
        };
}
