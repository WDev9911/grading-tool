namespace GradingTool.Application.DTOs;

public class RubricValidationDto
{
    public bool IsValid { get; set; }
    public decimal TotalScore { get; set; }
    public List<QuestionValidationDto> Questions { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public class QuestionValidationDto
{
    public int QuestionId { get; set; }
    public int QuestionNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal QuestionMaxScore { get; set; }
    public decimal SumCriteriaScore { get; set; }
    public bool IsBalanced { get; set; }
}
