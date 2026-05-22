using GradingTool.Application.Common.Interfaces;
using GradingTool.Domain.Entities;
using GradingTool.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GradingTool.Infrastructure.Repositories;

public class GradeRepository : BaseRepository<Grade>, IGradeRepository
{
    public GradeRepository(AppDbContext context) : base(context) { }

    public async Task<Grade?> GetBySubmissionAndCriterionAsync(int submissionId, int criterionId)
        => await _context.Grades
            .FirstOrDefaultAsync(g => g.SubmissionId == submissionId && g.CriterionId == criterionId);

    public async Task<List<Grade>> GetBySubmissionIdAsync(int submissionId)
        => await _context.Grades
            .Include(g => g.Criterion)
                .ThenInclude(c => c.Question)
            .Where(g => g.SubmissionId == submissionId)
            .ToListAsync();

    public async Task<decimal> GetTotalScoreAsync(int submissionId)
        => await _context.Grades
            .Where(g => g.SubmissionId == submissionId)
            .SumAsync(g => g.Score);

    public async Task<int> GetGradeCountAsync(int submissionId)
        => await _context.Grades
            .CountAsync(g => g.SubmissionId == submissionId);

    public async Task DeleteBySubmissionIdAsync(int submissionId)
    {
        var grades = await _context.Grades
            .Where(g => g.SubmissionId == submissionId)
            .ToListAsync();
        _context.Grades.RemoveRange(grades);
        await _context.SaveChangesAsync();
    }
}
