using GradingTool.Application.Common.Interfaces;
using GradingTool.Domain.Entities;
using GradingTool.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GradingTool.Infrastructure.Repositories;

public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
{
    public QuestionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Question>> GetAllWithCriteriaAsync()
        => await _context.Questions
            .Include(q => q.Criteria.OrderBy(c => c.OrderIndex))
            .OrderBy(q => q.OrderIndex)
            .ToListAsync();

    public async Task<Question?> GetByIdWithCriteriaAsync(int id)
        => await _context.Questions
            .Include(q => q.Criteria.OrderBy(c => c.OrderIndex))
            .FirstOrDefaultAsync(q => q.Id == id);

    public async Task<bool> ExistsByQuestionNoAsync(int questionNo, int? excludeId = null)
    {
        var query = _context.Questions.Where(q => q.QuestionNo == questionNo);
        if (excludeId.HasValue)
            query = query.Where(q => q.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<bool> HasGradesAsync(int questionId)
        => await _context.Grades
            .AnyAsync(g => g.Criterion.QuestionId == questionId);
}
