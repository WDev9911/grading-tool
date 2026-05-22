using GradingTool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GradingTool.API.Controllers;

[ApiController]
[Route("api/export")]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;

    public ExportController(IExportService exportService)
    {
        _exportService = exportService;
    }

    /// <summary>Xuất file Excel danh sách điểm (2 sheet: tổng quan + chi tiết tiêu chí)</summary>
    [HttpGet("excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var (fileBytes, fileName) = await _exportService.ExportGradesToExcelAsync();
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
