using System.Net;
using System.Text.Json;
using GraphCalc.Api.Responses;
using GraphCalc.Domain.Exceptions;

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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred.";
        string? errorCode = null;
        Dictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case ValidationException vex:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = vex.Message;
                errors = new Dictionary<string, string[]>(vex.Errors);
                break;
            case NotFoundException nfe:
                statusCode = (int)HttpStatusCode.NotFound;
                message = nfe.Message;
                break;
            case ArgumentException aex:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = aex.Message;
                break;
            case InvalidOperationException ioex:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = ioex.Message;
                break;
            default:
                logger.LogError(exception, "Unhandled exception");
                break;
        }

        var response = errors != null
            ? new ErrorResponse(statusCode, message, errors)
            : new ErrorResponse(statusCode, message, errorCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}
