namespace GradingTool.Application.Services.Interfaces;

public interface IExportService
{
    Task<(byte[] FileBytes, string FileName)> ExportGradesToExcelAsync();
}
