using System.Text.Json;
using AppValidationException = GradingTool.Application.Common.Exceptions.ValidationException;
using GradingTool.Application.Common.Exceptions;

namespace GradingTool.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, type) = exception switch
        {
            NotFoundException             => (404, "Not Found",             "https://tools.ietf.org/html/rfc7231#section-6.5.4"),
            ConflictException             => (409, "Conflict",              "https://tools.ietf.org/html/rfc7231#section-6.5.8"),
            AppValidationException        => (400, "Validation Failed",     "https://tools.ietf.org/html/rfc7231#section-6.5.1"),
            BadRequestException           => (400, "Bad Request",           "https://tools.ietf.org/html/rfc7231#section-6.5.1"),
            UnauthorizedAccessException   => (401, "Unauthorized",          "https://tools.ietf.org/html/rfc7235#section-3.1"),
            _                             => (500, "Internal Server Error", "https://tools.ietf.org/html/rfc7231#section-6.6.1")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var instance = $"{context.Request.Path}{context.Request.QueryString}";
        var traceId = context.TraceIdentifier;

        object problem;

        if (exception is AppValidationException validationEx)
        {
            problem = new
            {
                type,
                title,
                status = statusCode,
                detail = exception.Message,
                instance,
                traceId,
                errors = validationEx.Errors
            };
        }
        else
        {
            problem = new
            {
                type,
                title,
                status = statusCode,
                detail = exception.Message,
                instance,
                traceId
            };
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
    }
}
