using GradingTool.API.Common;
using GradingTool.Application.DTOs;
using GradingTool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GradingTool.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RubricController : ControllerBase
{
    private readonly IRubricValidationService _rubricValidationService;

    public RubricController(IRubricValidationService rubricValidationService)
    {
        _rubricValidationService = rubricValidationService;
    }

    [HttpGet("validate")]
    public async Task<ActionResult<ApiResponse<RubricValidationDto>>> Validate()
    {
        var result = await _rubricValidationService.ValidateAsync();
        return Ok(ApiResponse<RubricValidationDto>.Ok(result));
    }
}
