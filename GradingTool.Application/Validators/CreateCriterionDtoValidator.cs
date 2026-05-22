using FluentValidation;
using GradingTool.Application.DTOs;

namespace GradingTool.Application.Validators;

public class CreateCriterionDtoValidator : AbstractValidator<CreateCriterionDto>
{
    public CreateCriterionDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(20).WithMessage("Code max length is 20");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(300).WithMessage("Name max length is 300");

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage("MaxScore must be > 0");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("OrderIndex must be >= 0");
    }
}
