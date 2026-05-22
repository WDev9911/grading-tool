namespace GradingTool.Application.DTOs;

public class SubmissionGradingDto
{
    public int SubmissionId { get; set; }
    public string StudentCode { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal? TotalScore { get; set; }
    public decimal MaxTotalScore { get; set; }
    public string? GeneralComment { get; set; }
    public DateTime? GradedAt { get; set; }
    public List<QuestionGradingDto> Questions { get; set; } = new();
}
