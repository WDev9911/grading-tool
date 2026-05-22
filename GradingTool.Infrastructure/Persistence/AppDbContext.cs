using GradingTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GradingTool.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Question> Questions { get; set; }
    public DbSet<Criterion> Criteria { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Grade> Grades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Question>().HasData(
            new Question { Id = 1, QuestionNo = 1, Title = "Class Diagram", MaxScore = 3.0m, OrderIndex = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Question { Id = 2, QuestionNo = 2, Title = "Sequence Diagram", MaxScore = 4.0m, OrderIndex = 2, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Question { Id = 3, QuestionNo = 3, Title = "Activity/Statechart Diagram", MaxScore = 3.0m, OrderIndex = 3, CreatedAt = seedDate, UpdatedAt = seedDate }
        );

        modelBuilder.Entity<Criterion>().HasData(
            new Criterion { Id = 1,  QuestionId = 1, Code = "1.1", Name = "Đúng các class chính",        MaxScore = 1.0m, OrderIndex = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Criterion { Id = 2,  QuestionId = 1, Code = "1.2", Name = "Quan hệ giữa class",           MaxScore = 1.0m, OrderIndex = 2, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Criterion { Id = 3,  QuestionId = 1, Code = "1.3", Name = "Thuộc tính/method",            MaxScore = 0.5m, OrderIndex = 3, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Criterion { Id = 4,  QuestionId = 1, Code = "1.4", Name = "Phần giải thích",              MaxScore = 0.5m, OrderIndex = 4, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Criterion { Id = 5,  QuestionId = 2, Code = "2.1", Name = "Actor & Object đúng",          MaxScore = 1.0m, OrderIndex = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Criterion { Id = 6,  QuestionId = 2, Code = "2.2", Name = "Messages đúng thứ tự",         MaxScore = 1.5m, OrderIndex = 2, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Criterion { Id = 7,  QuestionId = 2, Code = "2.3", Name = "Xử lý điều kiện alt/opt",      MaxScore = 1.0m, OrderIndex = 3, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Criterion { Id = 8,  QuestionId = 2, Code = "2.4", Name = "Phần giải thích",              MaxScore = 0.5m, OrderIndex = 4, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Criterion { Id = 9,  QuestionId = 3, Code = "3.1", Name = "Các state/activity đúng",      MaxScore = 1.5m, OrderIndex = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Criterion { Id = 10, QuestionId = 3, Code = "3.2", Name = "Transitions đúng",             MaxScore = 1.0m, OrderIndex = 2, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Criterion { Id = 11, QuestionId = 3, Code = "3.3", Name = "Phần giải thích",              MaxScore = 0.5m, OrderIndex = 3, CreatedAt = seedDate, UpdatedAt = seedDate }
        );
    }
}
