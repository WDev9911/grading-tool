using GradingTool.Application.DTOs;

namespace GradingTool.Application.Services.Interfaces;

public interface ISubmissionService
{
    Task<BatchUploadResultDto> UploadAsync(List<UploadedFile> files, bool overwrite);
    Task<PagedResult<SubmissionListItemDto>> GetPagedAsync(SubmissionFilterDto filter);
    Task<SubmissionDetailDto?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
    Task<SubmissionContentDto> GetContentAsync(int id);
    Task<List<SubmissionImageDto>> GetImagesMetadataAsync(int id, string baseUrl);
    Task<(Stream Stream, string ContentType, string FileName)> GetImageStreamAsync(int id, string fileName);
    Task<(Stream Stream, string FileName)> GetOriginalFileStreamAsync(int id);
}
