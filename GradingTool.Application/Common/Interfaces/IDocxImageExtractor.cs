namespace GradingTool.Application.Common.Interfaces;

public interface IDocxImageExtractor
{
    Task<List<(byte[] Bytes, string Extension)>> ExtractImagesAsync(string docxFilePath);
}
