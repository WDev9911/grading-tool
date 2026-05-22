namespace GradingTool.Application.DTOs;

public class GradeDto
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    public int CriterionId { get; set; }
    public string CriterionCode { get; set; } = null!;
    public decimal Score { get; set; }
    public string? Comment { get; set; }
    public DateTime UpdatedAt { get; set; }
}
