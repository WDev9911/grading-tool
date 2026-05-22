using FluentValidation;
using GradingTool.Application.DTOs;

namespace GradingTool.Application.Validators;

public class UpdateGeneralCommentDtoValidator : AbstractValidator<UpdateGeneralCommentDto>
{
    public UpdateGeneralCommentDtoValidator()
    {
        RuleFor(x => x.Comment)
            .MaximumLength(2000).WithMessage("Nhận xét chung không quá 2000 ký tự");
    }
}
