namespace GradingTool.Application.DTOs;

public class SubmissionContentDto
{
    public int SubmissionId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int ImageCount { get; set; }
}
