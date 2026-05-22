namespace GradingTool.Application.DTOs;

public class UploadFileResultDto
{
    public string FileName { get; set; } = null!;
    public string Status { get; set; } = null!;   // "success" | "failed" | "duplicate"
    public int? SubmissionId { get; set; }
    public string? StudentCode { get; set; }
    public string? StudentName { get; set; }
    public string? Reason { get; set; }
    public int? ExistingSubmissionId { get; set; }
}
