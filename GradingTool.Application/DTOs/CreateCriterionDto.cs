namespace GradingTool.Application.DTOs;

public class CreateCriterionDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public int OrderIndex { get; set; }
}
