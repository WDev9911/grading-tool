namespace GradingTool.Domain.Entities;

public class Criterion
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Question Question { get; set; } = null!;
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
