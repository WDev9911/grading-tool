using GradingTool.Application.Common.Interfaces;
using GradingTool.Application.Common.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GradingTool.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly IDocxImageExtractor _imageExtractor;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        IOptions<StorageSettings> settings,
        IDocxImageExtractor imageExtractor,
        ILogger<LocalFileStorageService> logger)
    {
        var configured = settings.Value.BasePath;
        _basePath = Path.IsPathRooted(configured)
            ? configured
            : Path.GetFullPath(configured);
        _imageExtractor = imageExtractor;
        _logger = logger;
    }

    public async Task<string> SaveSubmissionFileAsync(int submissionId, Stream fileStream, string originalFileName)
    {
        var dir = SubmissionDir(submissionId);
        Directory.CreateDirectory(dir);

        var dest = Path.Combine(dir, "original.docx");
        await using var fs = File.Create(dest);
        await fileStream.CopyToAsync(fs);

        return $"submissions/{submissionId}/original.docx";
    }

    public async Task<List<string>> ExtractAndSaveImagesAsync(int submissionId, string docxFilePath)
    {
        var images = await _imageExtractor.ExtractImagesAsync(docxFilePath);
        var paths = new List<string>();

        if (images.Count == 0) return paths;

        var imagesDir = Path.Combine(SubmissionDir(submissionId), "images");
        Directory.CreateDirectory(imagesDir);

        for (var i = 0; i < images.Count; i++)
        {
            var (bytes, ext) = images[i];
            var fileName = $"image{i + 1}{ext}";
            await File.WriteAllBytesAsync(Path.Combine(imagesDir, fileName), bytes);
            paths.Add($"submissions/{submissionId}/images/{fileName}");
        }

        return paths;
    }

    public Task DeleteSubmissionFolderAsync(int submissionId)
    {
        var dir = SubmissionDir(submissionId);
        if (Directory.Exists(dir))
            Directory.Delete(dir, recursive: true);
        return Task.CompletedTask;
    }

    public string GetAbsolutePath(string relativePath)
        => Path.Combine(_basePath, relativePath.Replace('/', Path.DirectorySeparatorChar));

    public bool FileExists(string relativePath)
        => File.Exists(GetAbsolutePath(relativePath));

    public int GetImageCount(int submissionId)
    {
        var imagesDir = Path.Combine(SubmissionDir(submissionId), "images");
        return Directory.Exists(imagesDir) ? Directory.GetFiles(imagesDir).Length : 0;
    }

    public List<(string FileName, long SizeBytes)> GetImageFiles(int submissionId)
    {
        var imagesDir = Path.Combine(SubmissionDir(submissionId), "images");
        if (!Directory.Exists(imagesDir)) return [];

        return Directory.GetFiles(imagesDir)
            .OrderBy(f => NaturalSortKey(Path.GetFileName(f)))
            .Select(f => (Path.GetFileName(f), new FileInfo(f).Length))
            .ToList();
    }

    public Stream OpenImageFile(int submissionId, string fileName)
    {
        var path = Path.Combine(SubmissionDir(submissionId), "images", fileName);
        if (!File.Exists(path))
            throw new FileNotFoundException("Image not found", path);
        return File.OpenRead(path);
    }

    public Stream OpenDocxFile(int submissionId)
    {
        var path = Path.Combine(SubmissionDir(submissionId), "original.docx");
        if (!File.Exists(path))
            throw new FileNotFoundException("Docx not found", path);
        return File.OpenRead(path);
    }

    private string SubmissionDir(int submissionId)
        => Path.Combine(_basePath, "submissions", submissionId.ToString());

    // Natural sort: "image10" after "image9"
    private static string NaturalSortKey(string name)
    {
        var result = System.Text.RegularExpressions.Regex.Replace(
            name, @"\d+", m => m.Value.PadLeft(10, '0'));
        return result;
    }
}
