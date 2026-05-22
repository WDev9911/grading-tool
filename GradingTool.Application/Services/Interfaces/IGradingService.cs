using GradingTool.Application.DTOs;

namespace GradingTool.Application.Services.Interfaces;

public interface IGradingService
{
    Task<SubmissionGradingDto> GetGradingDetailAsync(int submissionId);
    Task<GradeDto> UpsertGradeAsync(int submissionId, int criterionId, UpsertGradeDto dto);
    Task UpdateGeneralCommentAsync(int submissionId, string? comment);
    Task<SubmissionGradingDto> MarkAsGradedAsync(int submissionId);
    Task ResetGradesAsync(int submissionId);
}
