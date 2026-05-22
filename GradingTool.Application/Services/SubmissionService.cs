using AutoMapper;
using GradingTool.Application.Common.Exceptions;
using GradingTool.Application.Common.Interfaces;
using GradingTool.Application.Common.Settings;
using GradingTool.Application.DTOs;
using GradingTool.Application.Services.Interfaces;
using GradingTool.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GradingTool.Application.Services;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly IDocxParserService _docxParser;
    private readonly IDocxTextExtractor _docxTextExtractor;
    private readonly IMapper _mapper;
    private readonly ILogger<SubmissionService> _logger;
    private readonly int _maxFileSizeMB;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IFileStorageService fileStorage,
        IDocxParserService docxParser,
        IDocxTextExtractor docxTextExtractor,
        IMapper mapper,
        IOptions<StorageSettings> storageSettings,
        ILogger<SubmissionService> logger)
    {
        _submissionRepository = submissionRepository;
        _fileStorage = fileStorage;
        _docxParser = docxParser;
        _docxTextExtractor = docxTextExtractor;
        _mapper = mapper;
        _logger = logger;
        _maxFileSizeMB = storageSettings.Value.MaxFileSizeMB;
    }

    public async Task<BatchUploadResultDto> UploadAsync(List<UploadedFile> files, bool overwrite)
    {
        if (files.Count == 0)
            throw new BadRequestException("Phải upload ít nhất 1 file");
        if (files.Count > 100)
            throw new BadRequestException("Tối đa 100 file mỗi lần upload");

        var maxBytes = _maxFileSizeMB * 1024L * 1024L;
        var results = new List<UploadFileResultDto>();

        foreach (var file in files)
            results.Add(await ProcessFileAsync(file, overwrite, maxBytes));

        return new BatchUploadResultDto
        {
            TotalFiles = files.Count,
            SuccessCount = results.Count(r => r.Status == "success"),
            FailedCount = results.Count(r => r.Status == "failed"),
            DuplicateCount = results.Count(r => r.Status == "duplicate"),
            Results = results
        };
    }

    private async Task<UploadFileResultDto> ProcessFileAsync(UploadedFile file, bool overwrite, long maxBytes)
    {
        var fileName = Path.GetFileName(file.FileName);

        try
        {
            // Path traversal guard
            if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
                return Fail(fileName, "Tên file không hợp lệ");

            if (!fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                return Fail(fileName, "File phải là .docx");

            if (file.Size == 0)
                return Fail(fileName, "File rỗng");

            if (file.Size > maxBytes)
                return Fail(fileName, $"File vượt quá kích thước tối đa {_maxFileSizeMB}MB");

            string studentName, studentCode;
            try { (studentName, studentCode) = _docxParser.ParseFileName(fileName); }
            catch (BadRequestException ex) { return Fail(fileName, ex.Message); }

            // Check duplicate
            var existing = await _submissionRepository.GetByStudentCodeAsync(studentCode);
            if (existing != null)
            {
                if (!overwrite)
                    return new UploadFileResultDto
                    {
                        FileName = fileName,
                        Status = "duplicate",
                        StudentCode = studentCode,
                        StudentName = studentName,
                        ExistingSubmissionId = existing.Id,
                        Reason = $"MSSV {studentCode} đã có bài nộp (id={existing.Id})"
                    };

                await _fileStorage.DeleteSubmissionFolderAsync(existing.Id);
                await _submissionRepository.DeleteAsync(existing);
            }

            // Create record to get Id
            var submission = new Submission
            {
                StudentCode = studentCode,
                StudentName = studentName,
                FileName = fileName,
                FilePath = string.Empty,
                UploadedAt = DateTime.UtcNow,
                Status = SubmissionStatus.NotGraded
            };
            await _submissionRepository.AddAsync(submission);

            // Save original file
            var relativePath = await _fileStorage.SaveSubmissionFileAsync(
                submission.Id, file.Stream, fileName);
            submission.FilePath = relativePath;
            await _submissionRepository.UpdateAsync(submission);

            // Extract images — non-fatal
            try
            {
                var absPath = _fileStorage.GetAbsolutePath(relativePath);
                await _fileStorage.ExtractAndSaveImagesAsync(submission.Id, absPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Image extraction failed for submission {Id} ({FileName}), continuing",
                    submission.Id, fileName);
            }

            return new UploadFileResultDto
            {
                FileName = fileName,
                Status = "success",
                SubmissionId = submission.Id,
                StudentCode = studentCode,
                StudentName = studentName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing upload for {FileName}", fileName);
            return Fail(fileName, $"Lỗi xử lý: {ex.Message}");
        }
    }

    public async Task<PagedResult<SubmissionListItemDto>> GetPagedAsync(SubmissionFilterDto filter)
    {
        var (items, total) = await _submissionRepository.GetPagedAsync(
            filter.Status, filter.Search,
            filter.MinScore, filter.MaxScore,
            filter.SortBy, filter.SortOrder,
            filter.Page, filter.PageSize);

        return new PagedResult<SubmissionListItemDto>
        {
            Items = _mapper.Map<List<SubmissionListItemDto>>(items),
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<SubmissionDetailDto?> GetByIdAsync(int id)
    {
        var submission = await _submissionRepository.GetByIdWithGradesAsync(id);
        if (submission == null) return null;

        var dto = _mapper.Map<SubmissionDetailDto>(submission);
        dto.ImageCount = _fileStorage.GetImageCount(id);
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var submission = await _submissionRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Submission", id);

        await _fileStorage.DeleteSubmissionFolderAsync(id);
        await _submissionRepository.DeleteAsync(submission);
    }

    public async Task<SubmissionContentDto> GetContentAsync(int id)
    {
        var submission = await _submissionRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Submission", id);

        if (!_fileStorage.FileExists(submission.FilePath))
            throw new NotFoundException("File bài làm", id);

        var absPath = _fileStorage.GetAbsolutePath(submission.FilePath);
        var text = await _docxTextExtractor.ExtractTextAsync(absPath);

        return new SubmissionContentDto
        {
            SubmissionId = submission.Id,
            StudentCode = submission.StudentCode,
            StudentName = submission.StudentName,
            Text = text,
            ImageCount = _fileStorage.GetImageCount(id)
        };
    }

    public Task<List<SubmissionImageDto>> GetImagesMetadataAsync(int id, string baseUrl)
    {
        _ = _submissionRepository.GetByIdAsync(id);
        var files = _fileStorage.GetImageFiles(id);

        var dtos = files.Select(f => new SubmissionImageDto
        {
            FileName = f.FileName,
            SizeBytes = f.SizeBytes,
            Url = $"{baseUrl.TrimEnd('/')}/api/submissions/{id}/images/{f.FileName}"
        }).ToList();

        return Task.FromResult(dtos);
    }

    public async Task<(Stream Stream, string ContentType, string FileName)> GetImageStreamAsync(
        int id, string fileName)
    {
        // Path traversal guard
        if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
            throw new BadRequestException("Tên file không hợp lệ");

        _ = await _submissionRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Submission", id);

        var stream = _fileStorage.OpenImageFile(id, fileName);
        var contentType = ExtensionToContentType(Path.GetExtension(fileName));
        return (stream, contentType, fileName);
    }

    public async Task<(Stream Stream, string FileName)> GetOriginalFileStreamAsync(int id)
    {
        var submission = await _submissionRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Submission", id);

        if (!_fileStorage.FileExists(submission.FilePath))
            throw new NotFoundException("File bài làm", id);

        var stream = _fileStorage.OpenDocxFile(id);
        return (stream, submission.FileName);
    }

    private static string ExtensionToContentType(string ext) => ext.ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".bmp" => "image/bmp",
        ".tiff" or ".tif" => "image/tiff",
        ".emf" => "image/emf",
        ".wmf" => "image/wmf",
        _ => "application/octet-stream"
    };

    private static UploadFileResultDto Fail(string fileName, string reason) => new()
    {
        FileName = fileName,
        Status = "failed",
        Reason = reason
    };
}
