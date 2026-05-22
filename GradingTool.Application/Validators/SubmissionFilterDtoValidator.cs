using FluentValidation;
using GradingTool.Application.DTOs;
using GradingTool.Domain.Entities;

namespace GradingTool.Application.Validators;

public class SubmissionFilterDtoValidator : AbstractValidator<SubmissionFilterDto>
{
    private static readonly string[] ValidStatuses =
        Enum.GetNames<SubmissionStatus>();

    public SubmissionFilterDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be >= 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        When(x => x.SortOrder != null, () =>
            RuleFor(x => x.SortOrder)
                .Must(s => s == "asc" || s == "desc")
                .WithMessage("SortOrder must be 'asc' or 'desc'"));

        When(x => x.Status != null, () =>
            RuleFor(x => x.Status)
                .Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}"));
    }
}
