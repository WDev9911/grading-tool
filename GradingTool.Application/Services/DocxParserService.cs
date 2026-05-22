using System.Text.RegularExpressions;
using GradingTool.Application.Common.Exceptions;
using GradingTool.Application.Common.Interfaces;

namespace GradingTool.Application.Services;

public class DocxParserService : IDocxParserService
{
    private static readonly Regex FileNameRegex =
        new(@"^(.+)_(SE\d+)\.docx$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public (string StudentName, string StudentCode) ParseFileName(string fileName)
    {
        var match = FileNameRegex.Match(fileName);
        if (!match.Success)
            throw new BadRequestException(
                $"Tên file không đúng format <HọTên>_<MSSV>.docx: '{fileName}'");

        return (match.Groups[1].Value.Trim(), match.Groups[2].Value.ToUpper());
    }
}
