namespace GradingTool.Application.DTOs;

public class QuestionDto
{
    public int Id { get; set; }
    public int QuestionNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CriterionDto> Criteria { get; set; } = new();
    public decimal SumCriteriaScore { get; set; }
    public bool IsBalanced { get; set; }
}
