namespace GradingTool.Domain.Entities;

public class Submission
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.NotGraded;
    public decimal? TotalScore { get; set; }
    public string? GeneralComment { get; set; }
    public DateTime? GradedAt { get; set; }

    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}

public enum SubmissionStatus
{
    NotGraded,
    Grading,
    Graded
}
