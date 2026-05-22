using GradingTool.Application.DTOs;

namespace GradingTool.Application.Services.Interfaces;

public interface IRubricValidationService
{
    Task<RubricValidationDto> ValidateAsync();
}
