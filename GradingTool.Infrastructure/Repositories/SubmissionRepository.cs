using GradingTool.Application.Common.Interfaces;
using GradingTool.Domain.Entities;
using GradingTool.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GradingTool.Infrastructure.Repositories;

public class SubmissionRepository : BaseRepository<Submission>, ISubmissionRepository
{
    public SubmissionRepository(AppDbContext context) : base(context) { }

    public async Task<Submission?> GetByStudentCodeAsync(string studentCode)
        => await _context.Submissions
            .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

    public async Task<Submission?> GetByIdWithGradesAsync(int id)
        => await _context.Submissions
            .Include(s => s.Grades)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<Submission>> GetAllWithGradesAsync()
        => await _context.Submissions
            .Include(s => s.Grades)
                .ThenInclude(g => g.Criterion)
            .OrderBy(s => s.StudentCode)
            .ToListAsync();

    public async Task<bool> ExistsByStudentCodeAsync(string studentCode)
        => await _context.Submissions.AnyAsync(s => s.StudentCode == studentCode);

    public async Task<(List<Submission> Items, int Total)> GetPagedAsync(
        string? status, string? search,
        decimal? minScore, decimal? maxScore,
        string? sortBy, string? sortOrder,
        int page, int pageSize)
    {
        var query = _context.Submissions.AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<SubmissionStatus>(status, true, out var statusEnum))
            query = query.Where(s => s.Status == statusEnum);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(s =>
                s.StudentCode.Contains(search) ||
                s.StudentName.Contains(search));

        if (minScore.HasValue)
            query = query.Where(s => s.TotalScore >= minScore);

        if (maxScore.HasValue)
            query = query.Where(s => s.TotalScore <= maxScore);

        bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortBy?.ToLower() switch
        {
            "studentcode"  => desc ? query.OrderByDescending(s => s.StudentCode)  : query.OrderBy(s => s.StudentCode),
            "studentname"  => desc ? query.OrderByDescending(s => s.StudentName)  : query.OrderBy(s => s.StudentName),
            "totalscore"   => desc ? query.OrderByDescending(s => s.TotalScore)   : query.OrderBy(s => s.TotalScore),
            "uploadedat"   => desc ? query.OrderByDescending(s => s.UploadedAt)   : query.OrderBy(s => s.UploadedAt),
            _ => query.OrderByDescending(s => s.UploadedAt)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
