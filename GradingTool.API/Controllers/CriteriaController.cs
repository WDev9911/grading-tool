using GradingTool.API.Common;
using GradingTool.Application.DTOs;
using GradingTool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GradingTool.API.Controllers;

[ApiController]
public class CriteriaController : ControllerBase
{
    private readonly ICriterionService _criterionService;

    public CriteriaController(ICriterionService criterionService)
    {
        _criterionService = criterionService;
    }

    [HttpGet("api/questions/{questionId:int}/criteria")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CriterionDto>>>> GetByQuestion(int questionId)
    {
        var criteria = await _criterionService.GetByQuestionIdAsync(questionId);
        return Ok(ApiResponse<IEnumerable<CriterionDto>>.Ok(criteria));
    }

    [HttpPost("api/questions/{questionId:int}/criteria")]
    public async Task<ActionResult<ApiResponse<CriterionDto>>> Create(int questionId, [FromBody] CreateCriterionDto dto)
    {
        var criterion = await _criterionService.CreateAsync(questionId, dto);
        return StatusCode(201, ApiResponse<CriterionDto>.Ok(criterion));
    }

    [HttpPut("api/criteria/{id:int}")]
    public async Task<ActionResult<ApiResponse<CriterionDto>>> Update(int id, [FromBody] UpdateCriterionDto dto)
    {
        var criterion = await _criterionService.UpdateAsync(id, dto);
        return Ok(ApiResponse<CriterionDto>.Ok(criterion));
    }

    [HttpDelete("api/criteria/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _criterionService.DeleteAsync(id);
        return NoContent();
    }
}
