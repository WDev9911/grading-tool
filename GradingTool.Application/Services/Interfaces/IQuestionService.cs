using GradingTool.Application.DTOs;

namespace GradingTool.Application.Services.Interfaces;

public interface IQuestionService
{
    Task<IEnumerable<QuestionDto>> GetAllAsync();
    Task<QuestionDto> GetByIdAsync(int id);
    Task<QuestionDto> CreateAsync(CreateQuestionDto dto);
    Task<QuestionDto> UpdateAsync(int id, UpdateQuestionDto dto);
    Task DeleteAsync(int id);
}
