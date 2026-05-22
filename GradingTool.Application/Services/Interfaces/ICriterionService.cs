using GradingTool.Application.DTOs;

namespace GradingTool.Application.Services.Interfaces;

public interface ICriterionService
{
    Task<IEnumerable<CriterionDto>> GetByQuestionIdAsync(int questionId);
    Task<CriterionDto> CreateAsync(int questionId, CreateCriterionDto dto);
    Task<CriterionDto> UpdateAsync(int id, UpdateCriterionDto dto);
    Task DeleteAsync(int id);
}
