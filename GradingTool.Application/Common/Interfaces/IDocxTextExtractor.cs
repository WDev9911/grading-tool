namespace GradingTool.Application.Common.Interfaces;

public interface IDocxTextExtractor
{
    Task<string> ExtractTextAsync(string docxFilePath);
}
