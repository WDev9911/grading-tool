using GradingTool.Application.Common.Interfaces;
using GradingTool.Domain.Entities;
using GradingTool.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GradingTool.Infrastructure.Repositories;

public class CriterionRepository : BaseRepository<Criterion>, ICriterionRepository
{
    public CriterionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Criterion>> GetByQuestionIdAsync(int questionId)
        => await _context.Criteria
            .Where(c => c.QuestionId == questionId)
            .OrderBy(c => c.OrderIndex)
            .ToListAsync();

    public async Task<bool> ExistsByCodeAsync(int questionId, string code, int? excludeId = null)
    {
        var query = _context.Criteria.Where(c => c.QuestionId == questionId && c.Code == code);
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public Task<bool> HasGradesAsync(int criterionId)
    {
        // Placeholder — implement when Grade entity is added
        return Task.FromResult(false);
    }
}
