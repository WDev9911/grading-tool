using ClosedXML.Excel;
using GradingTool.Application.Common.Interfaces;
using GradingTool.Application.Services.Interfaces;
using GradingTool.Domain.Entities;

namespace GradingTool.Application.Services;

public class ExportService : IExportService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ISubmissionRepository _submissionRepository;

    public ExportService(
        IQuestionRepository questionRepository,
        ISubmissionRepository submissionRepository)
    {
        _questionRepository = questionRepository;
        _submissionRepository = submissionRepository;
    }

    public async Task<(byte[] FileBytes, string FileName)> ExportGradesToExcelAsync()
    {
        var questions = (await _questionRepository.GetAllWithCriteriaAsync())
            .OrderBy(q => q.OrderIndex).ToList();
        var criteria = questions.SelectMany(q => q.Criteria.OrderBy(c => c.OrderIndex)).ToList();
        var submissions = await _submissionRepository.GetAllWithGradesAsync();

        using var workbook = new XLWorkbook();
        BuildOverviewSheet(workbook, questions, submissions);
        BuildDetailSheet(workbook, criteria, submissions);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var bytes = stream.ToArray();
        var fileName = $"SWD392_Grades_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return (bytes, fileName);
    }

    // ─── Sheet 1: Danh sách điểm (overview) ───────────────────────────────────

    private static void BuildOverviewSheet(
        XLWorkbook workbook,
        List<Question> questions,
        List<Submission> submissions)
    {
        var ws = workbook.Worksheets.Add("Danh sách điểm");

        // Fixed columns: STT(1) MSSV(2) HọTên(3) ... Questions ... Tổng Status Nhận xét Ngày chấm
        int questionStartCol = 4;
        int totalCol = questionStartCol + questions.Count;
        int statusCol = totalCol + 1;
        int commentCol = statusCol + 1;
        int gradedAtCol = commentCol + 1;
        int totalColumns = gradedAtCol;

        // ── Row 1: Title ──────────────────────────────────────────────────────
        ws.Cell(1, 1).Value = "DANH SÁCH ĐIỂM THI - SWD392";
        ws.Range(1, 1, 1, totalColumns).Merge();
        ApplyTitleStyle(ws.Cell(1, 1));

        // ── Row 2: Header ─────────────────────────────────────────────────────
        ws.Cell(2, 1).Value = "STT";
        ws.Cell(2, 2).Value = "MSSV";
        ws.Cell(2, 3).Value = "Họ tên";

        for (int i = 0; i < questions.Count; i++)
        {
            var q = questions[i];
            ws.Cell(2, questionStartCol + i).Value = $"Câu {q.QuestionNo} ({q.MaxScore}đ)";
        }

        decimal maxTotal = questions.Sum(q => q.MaxScore);
        ws.Cell(2, totalCol).Value = $"Tổng ({maxTotal}đ)";
        ws.Cell(2, statusCol).Value = "Trạng thái";
        ws.Cell(2, commentCol).Value = "Nhận xét chung";
        ws.Cell(2, gradedAtCol).Value = "Ngày chấm";

        ApplyHeaderStyle(ws.Range(2, 1, 2, totalColumns));

        // ── Rows 3+: Data ─────────────────────────────────────────────────────
        int row = 3;
        foreach (var sub in submissions)
        {
            var gradeMap = sub.Grades.ToDictionary(g => g.CriterionId);

            ws.Cell(row, 1).Value = row - 2;
            ws.Cell(row, 2).Value = sub.StudentCode;
            ws.Cell(row, 3).Value = sub.StudentName;

            for (int i = 0; i < questions.Count; i++)
            {
                var questionScore = sub.Grades
                    .Where(g => g.Criterion?.QuestionId == questions[i].Id)
                    .Sum(g => (decimal?)g.Score);
                if (questionScore.HasValue)
                {
                    var cell = ws.Cell(row, questionStartCol + i);
                    cell.Value = questionScore.Value;
                    cell.Style.NumberFormat.Format = "0.00";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }

            // Total score
            var totalCell = ws.Cell(row, totalCol);
            if (sub.TotalScore.HasValue)
            {
                totalCell.Value = sub.TotalScore.Value;
                totalCell.Style.NumberFormat.Format = "0.00";
                totalCell.Style.Font.Bold = true;
                totalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                totalCell.Style.Fill.BackgroundColor = ScoreColor(sub.TotalScore.Value);
            }

            // Status
            var statusCell = ws.Cell(row, statusCol);
            var (statusText, statusColor) = StatusInfo(sub.Status);
            statusCell.Value = statusText;
            statusCell.Style.Font.FontColor = XLColor.FromHtml(statusColor);
            statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Comment
            if (!string.IsNullOrEmpty(sub.GeneralComment))
                ws.Cell(row, commentCol).Value = sub.GeneralComment;

            // Graded at
            if (sub.GradedAt.HasValue)
            {
                var dateCell = ws.Cell(row, gradedAtCol);
                dateCell.Value = sub.GradedAt.Value.ToLocalTime().ToString("dd/MM/yyyy");
                dateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            ApplyDataRowBorder(ws.Range(row, 1, row, totalColumns));
            row++;
        }

        if (row > 3)
            ws.Range(3, 1, row - 1, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        ws.Columns().AdjustToContents();
        ws.SheetView.FreezeRows(2);
    }

    // ─── Sheet 2: Chi tiết Rubric ──────────────────────────────────────────────

    private static void BuildDetailSheet(
        XLWorkbook workbook,
        List<Criterion> criteria,
        List<Submission> submissions)
    {
        var ws = workbook.Worksheets.Add("Chi tiết Rubric");

        int criteriaStartCol = 3;
        int totalCol = criteriaStartCol + criteria.Count;
        int totalColumns = totalCol;

        // ── Row 1: Title ──────────────────────────────────────────────────────
        ws.Cell(1, 1).Value = "CHI TIẾT ĐIỂM THEO TIÊU CHÍ - SWD392";
        ws.Range(1, 1, 1, totalColumns).Merge();
        ApplyTitleStyle(ws.Cell(1, 1));

        // ── Row 2: Header ─────────────────────────────────────────────────────
        ws.Cell(2, 1).Value = "MSSV";
        ws.Cell(2, 2).Value = "Họ tên";

        for (int i = 0; i < criteria.Count; i++)
        {
            var c = criteria[i];
            ws.Cell(2, criteriaStartCol + i).Value = $"{c.Code} ({c.MaxScore}đ)";
        }

        decimal maxTotal = criteria.Sum(c => c.MaxScore);
        ws.Cell(2, totalCol).Value = $"Tổng ({maxTotal}đ)";

        ApplyHeaderStyle(ws.Range(2, 1, 2, totalColumns));

        // ── Rows 3+: Data ─────────────────────────────────────────────────────
        int row = 3;
        foreach (var sub in submissions)
        {
            ws.Cell(row, 1).Value = sub.StudentCode;
            ws.Cell(row, 2).Value = sub.StudentName;

            for (int i = 0; i < criteria.Count; i++)
            {
                var grade = sub.Grades.FirstOrDefault(g => g.CriterionId == criteria[i].Id);
                if (grade != null)
                {
                    var cell = ws.Cell(row, criteriaStartCol + i);
                    cell.Value = grade.Score;
                    cell.Style.NumberFormat.Format = "0.00";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }

            var totalCell = ws.Cell(row, totalCol);
            if (sub.TotalScore.HasValue)
            {
                totalCell.Value = sub.TotalScore.Value;
                totalCell.Style.NumberFormat.Format = "0.00";
                totalCell.Style.Font.Bold = true;
                totalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                totalCell.Style.Fill.BackgroundColor = ScoreColor(sub.TotalScore.Value);
            }

            ApplyDataRowBorder(ws.Range(row, 1, row, totalColumns));
            row++;
        }

        ws.Columns().AdjustToContents();
        ws.SheetView.Freeze(1, 2); // freeze row 1 + 2 columns
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static void ApplyTitleStyle(IXLCell cell)
    {
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontSize = 14;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }

    private static void ApplyHeaderStyle(IXLRange range)
    {
        range.Style.Font.Bold = true;
        range.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    }

    private static void ApplyDataRowBorder(IXLRange range)
    {
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    }

    private static XLColor ScoreColor(decimal score) => score switch
    {
        < 4m => XLColor.FromHtml("#FFCCCC"),
        < 7m => XLColor.FromHtml("#FFF4CC"),
        _ => XLColor.FromHtml("#CCFFCC")
    };

    private static (string Text, string HexColor) StatusInfo(SubmissionStatus status) => status switch
    {
        SubmissionStatus.Graded => ("Đã chấm", "#00B050"),
        SubmissionStatus.Grading => ("Đang chấm", "#ED7D31"),
        _ => ("Chưa chấm", "#C00000")
    };
}
