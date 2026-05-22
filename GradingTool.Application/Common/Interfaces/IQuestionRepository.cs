using GradingTool.Domain.Entities;

namespace GradingTool.Application.Common.Interfaces;

public interface IQuestionRepository : IBaseRepository<Question>
{
    Task<IEnumerable<Question>> GetAllWithCriteriaAsync();
    Task<Question?> GetByIdWithCriteriaAsync(int id);
    Task<bool> ExistsByQuestionNoAsync(int questionNo, int? excludeId = null);
    Task<bool> HasGradesAsync(int questionId);
}
