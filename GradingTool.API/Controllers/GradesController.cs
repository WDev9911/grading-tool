using GradingTool.API.Common;
using GradingTool.Application.DTOs;
using GradingTool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GradingTool.API.Controllers;

[ApiController]
[Route("api/submissions")]
public class GradesController : ControllerBase
{
    private readonly IGradingService _gradingService;

    public GradesController(IGradingService gradingService)
    {
        _gradingService = gradingService;
    }

    /// <summary>Lấy chi tiết chấm điểm theo cấu trúc rubric phân cấp</summary>
    [HttpGet("{id:int}/grades")]
    public async Task<ActionResult<ApiResponse<SubmissionGradingDto>>> GetGradingDetail(int id)
    {
        var result = await _gradingService.GetGradingDetailAsync(id);
        return Ok(ApiResponse<SubmissionGradingDto>.Ok(result));
    }

    /// <summary>Upsert điểm 1 tiêu chí (auto-save)</summary>
    [HttpPut("{id:int}/grades/{criterionId:int}")]
    public async Task<ActionResult<ApiResponse<GradeDto>>> UpsertGrade(
        int id, int criterionId, [FromBody] UpsertGradeDto dto)
    {
        var result = await _gradingService.UpsertGradeAsync(id, criterionId, dto);
        return Ok(ApiResponse<GradeDto>.Ok(result));
    }

    /// <summary>Cập nhật nhận xét chung cho bài</summary>
    [HttpPut("{id:int}/general-comment")]
    public async Task<IActionResult> UpdateGeneralComment(
        int id, [FromBody] UpdateGeneralCommentDto dto)
    {
        await _gradingService.UpdateGeneralCommentAsync(id, dto.Comment);
        return NoContent();
    }

    /// <summary>Đánh dấu đã chấm xong — yêu cầu đã có đủ tất cả tiêu chí</summary>
    [HttpPost("{id:int}/mark-graded")]
    public async Task<ActionResult<ApiResponse<SubmissionGradingDto>>> MarkAsGraded(int id)
    {
        var result = await _gradingService.MarkAsGradedAsync(id);
        return Ok(ApiResponse<SubmissionGradingDto>.Ok(result));
    }

    /// <summary>Reset toàn bộ điểm, đưa bài về trạng thái NotGraded</summary>
    [HttpDelete("{id:int}/grades")]
    public async Task<IActionResult> ResetGrades(int id)
    {
        await _gradingService.ResetGradesAsync(id);
        return NoContent();
    }
}
