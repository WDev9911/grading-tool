using FluentValidation;
using GradingTool.Application.DTOs;

namespace GradingTool.Application.Validators;

public class UpdateQuestionDtoValidator : AbstractValidator<UpdateQuestionDto>
{
    public UpdateQuestionDtoValidator()
    {
        RuleFor(x => x.QuestionNo)
            .GreaterThan(0).WithMessage("QuestionNo must be > 0");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title max length is 200");

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage("MaxScore must be > 0");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("OrderIndex must be >= 0");
    }
}
