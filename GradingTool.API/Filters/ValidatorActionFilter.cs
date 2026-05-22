using Microsoft.AspNetCore.Mvc.Filters;
using AppValidationException = GradingTool.Application.Common.Exceptions.ValidationException;

namespace GradingTool.API.Filters;

public class ValidatorActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(kvp => kvp.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            throw new AppValidationException(errors);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
