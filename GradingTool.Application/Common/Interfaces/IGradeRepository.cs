using GradingTool.Domain.Entities;

namespace GradingTool.Application.Common.Interfaces;

public interface IGradeRepository : IBaseRepository<Grade>
{
    Task<Grade?> GetBySubmissionAndCriterionAsync(int submissionId, int criterionId);
    Task<List<Grade>> GetBySubmissionIdAsync(int submissionId);
    Task<decimal> GetTotalScoreAsync(int submissionId);
    Task<int> GetGradeCountAsync(int submissionId);
    Task DeleteBySubmissionIdAsync(int submissionId);
}
