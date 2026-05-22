namespace GradingTool.Application.DTOs;

public class CreateQuestionDto
{
    public int QuestionNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public int OrderIndex { get; set; }
}
