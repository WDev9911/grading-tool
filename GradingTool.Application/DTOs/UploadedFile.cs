namespace GradingTool.Application.DTOs;

public class UploadedFile
{
    public string FileName { get; init; } = null!;
    public long Size { get; init; }
    public Stream Stream { get; init; } = null!;
}
