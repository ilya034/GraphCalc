using GraphCalc.Api.Responses;
using GraphCalc.Domain.Exceptions;
using System.Net;

namespace GraphCalc.App.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            EntityNotFoundException ex => HandleEntityNotFoundException(context, ex),
            ExpressionEvaluationException ex => HandleExpressionEvaluationException(context, ex),
            InvalidGraphParametersException ex => HandleInvalidGraphParametersException(context, ex),
            ArgumentNullException ex => HandleArgumentNullException(context, ex),
            ArgumentException ex => HandleArgumentException(context, ex),
            KeyNotFoundException ex => HandleKeyNotFoundException(context, ex),
            System.InvalidOperationException ex => HandleSystemInvalidOperationException(context, ex),
            _ => HandleGeneralException(context, exception)
        };

        LogException(exception);
        return context.Response.WriteAsJsonAsync(response);
    }

    private ErrorResponse HandleEntityNotFoundException(HttpContext context, EntityNotFoundException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        logger.LogWarning(ex, "Entity not found: {EntityType} with ID {EntityId}", ex.EntityType, ex.EntityId);
        return new ErrorResponse(
            (int)HttpStatusCode.NotFound,
            ex.Message,
            ex.ErrorCode
        );
    }

    private ErrorResponse HandleExpressionEvaluationException(HttpContext context, ExpressionEvaluationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        logger.LogWarning(ex, "Expression evaluation failed: {Expression}", ex.Expression);
        return new ErrorResponse(
            (int)HttpStatusCode.BadRequest,
            ex.Message,
            ex.ErrorCode
        );
    }

    private ErrorResponse HandleInvalidGraphParametersException(HttpContext context, InvalidGraphParametersException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        logger.LogWarning(ex, "Invalid graph parameters: {Message}", ex.Message);
        return new ErrorResponse(
            (int)HttpStatusCode.BadRequest,
            ex.Message,
            ex.ErrorCode
        );
    }

    private ErrorResponse HandleArgumentNullException(HttpContext context, ArgumentNullException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        logger.LogWarning(ex, "Null argument provided: {ParamName}", ex.ParamName);
        return new ErrorResponse(
            (int)HttpStatusCode.BadRequest,
            $"Required parameter is missing: {ex.ParamName}",
            "INVALID_ARGUMENT"
        );
    }

    private ErrorResponse HandleArgumentException(HttpContext context, ArgumentException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        logger.LogWarning(ex, "Invalid argument: {Message}", ex.Message);
        return new ErrorResponse(
            (int)HttpStatusCode.BadRequest,
            ex.Message,
            "VALIDATION_ERROR"
        );
    }

    private ErrorResponse HandleKeyNotFoundException(HttpContext context, KeyNotFoundException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        logger.LogWarning(ex, "Key not found: {Message}", ex.Message);
        return new ErrorResponse(
            (int)HttpStatusCode.NotFound,
            ex.Message,
            "NOT_FOUND"
        );
    }

    private ErrorResponse HandleSystemInvalidOperationException(HttpContext context, System.InvalidOperationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
        return new ErrorResponse(
            (int)HttpStatusCode.BadRequest,
            ex.Message,
            "OPERATION_FAILED"
        );
    }

    private ErrorResponse HandleGeneralException(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        logger.LogError(ex, "Unhandled exception occurred");
        return new ErrorResponse(
            (int)HttpStatusCode.InternalServerError,
            "An unexpected error occurred. Please try again later.",
            "INTERNAL_ERROR"
        );
    }

    private void LogException(Exception ex)
    {
        if (ex is DomainException domainEx)
        {
            logger.LogWarning(ex, "Domain exception: {ErrorCode}", domainEx.ErrorCode);
        }
        else if (ex is ArgumentException or ArgumentNullException)
        {
            logger.LogWarning(ex, "Argument validation error");
        }
        else
        {
            logger.LogError(ex, "Unexpected exception");
        }
    }
}

/// <summary>
/// Расширения для регистрации middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
