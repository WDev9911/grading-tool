namespace GradingTool.Application.DTOs;

public class QuestionGradingDto
{
    public int QuestionId { get; set; }
    public int QuestionNo { get; set; }
    public string Title { get; set; } = null!;
    public decimal MaxScore { get; set; }
    public decimal? AchievedScore { get; set; }
    public List<CriterionGradingDto> Criteria { get; set; } = new();
}
