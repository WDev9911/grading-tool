using GradingTool.API.Common;
using GradingTool.Application.DTOs;
using GradingTool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GradingTool.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<QuestionDto>>>> GetAll()
    {
        var questions = await _questionService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<QuestionDto>>.Ok(questions));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<QuestionDto>>> GetById(int id)
    {
        var question = await _questionService.GetByIdAsync(id);
        return Ok(ApiResponse<QuestionDto>.Ok(question));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<QuestionDto>>> Create([FromBody] CreateQuestionDto dto)
    {
        var question = await _questionService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = question.Id },
            ApiResponse<QuestionDto>.Ok(question));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<QuestionDto>>> Update(int id, [FromBody] UpdateQuestionDto dto)
    {
        var question = await _questionService.UpdateAsync(id, dto);
        return Ok(ApiResponse<QuestionDto>.Ok(question));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _questionService.DeleteAsync(id);
        return NoContent();
    }
}
