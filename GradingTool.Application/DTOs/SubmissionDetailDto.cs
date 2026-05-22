namespace GradingTool.Application.DTOs;

public class SubmissionDetailDto
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
    public string Status { get; set; } = null!;
    public decimal? TotalScore { get; set; }
    public DateTime? GradedAt { get; set; }
    public string FileName { get; set; } = null!;
    public string? GeneralComment { get; set; }
    public int ImageCount { get; set; }
}
