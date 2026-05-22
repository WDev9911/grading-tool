namespace GradingTool.Application.DTOs;

public class SubmissionImageDto
{
    public string FileName { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string Url { get; set; } = string.Empty;
}
