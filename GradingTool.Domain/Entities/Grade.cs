namespace GradingTool.Domain.Entities;

public class Grade
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    public int CriterionId { get; set; }
    public decimal Score { get; set; }
    public string? Comment { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Submission Submission { get; set; } = null!;
    public Criterion Criterion { get; set; } = null!;
}
