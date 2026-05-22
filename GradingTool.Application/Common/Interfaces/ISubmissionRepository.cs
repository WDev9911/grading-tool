using GradingTool.Domain.Entities;

namespace GradingTool.Application.Common.Interfaces;

public interface ISubmissionRepository : IBaseRepository<Submission>
{
    Task<Submission?> GetByStudentCodeAsync(string studentCode);
    Task<Submission?> GetByIdWithGradesAsync(int id);
    Task<List<Submission>> GetAllWithGradesAsync();
    Task<bool> ExistsByStudentCodeAsync(string studentCode);
    Task<(List<Submission> Items, int Total)> GetPagedAsync(
        string? status, string? search,
        decimal? minScore, decimal? maxScore,
        string? sortBy, string? sortOrder,
        int page, int pageSize);
}
