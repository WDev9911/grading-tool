namespace GradingTool.Application.DTOs;

public class BatchUploadResultDto
{
    public int TotalFiles { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public int DuplicateCount { get; set; }
    public List<UploadFileResultDto> Results { get; set; } = new();
}
