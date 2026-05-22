namespace GradingTool.Application.DTOs;

public class CriterionGradingDto
{
    public int CriterionId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal MaxScore { get; set; }
    public decimal? Score { get; set; }
    public string? Comment { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
