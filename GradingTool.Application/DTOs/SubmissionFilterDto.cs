namespace GradingTool.Application.DTOs;

public class SubmissionFilterDto
{
    public string? Status { get; set; }
    public string? Search { get; set; }
    public decimal? MinScore { get; set; }
    public decimal? MaxScore { get; set; }
    public string? SortBy { get; set; }     // "studentCode" | "studentName" | "totalScore" | "uploadedAt"
    public string? SortOrder { get; set; }  // "asc" | "desc"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
