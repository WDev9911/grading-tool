namespace GradingTool.Domain.Entities;

public class Question
{
    public int Id { get; set; }
    public int QuestionNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();
}
