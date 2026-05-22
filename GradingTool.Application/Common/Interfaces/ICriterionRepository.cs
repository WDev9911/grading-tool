using GradingTool.Domain.Entities;

namespace GradingTool.Application.Common.Interfaces;

public interface ICriterionRepository : IBaseRepository<Criterion>
{
    Task<IEnumerable<Criterion>> GetByQuestionIdAsync(int questionId);
    Task<bool> ExistsByCodeAsync(int questionId, string code, int? excludeId = null);
    Task<bool> HasGradesAsync(int criterionId);
}
