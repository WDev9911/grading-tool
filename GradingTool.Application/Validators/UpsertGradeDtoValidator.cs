using FluentValidation;
using GradingTool.Application.DTOs;

namespace GradingTool.Application.Validators;

public class UpsertGradeDtoValidator : AbstractValidator<UpsertGradeDto>
{
    public UpsertGradeDtoValidator()
    {
        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0).WithMessage("Điểm không được âm");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Nhận xét không quá 1000 ký tự");
    }
}
