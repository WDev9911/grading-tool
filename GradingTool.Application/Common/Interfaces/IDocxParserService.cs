namespace GradingTool.Application.Common.Interfaces;

public interface IDocxParserService
{
    (string StudentName, string StudentCode) ParseFileName(string fileName);
}
