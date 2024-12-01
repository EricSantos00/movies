using Microsoft.AspNetCore.Mvc;
using MoviesApi.Validations;

namespace MoviesApi.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            await HandleValidationException(context, exception);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process request");
            await HandleGenericException(context);
        }
    }

    private static async Task HandleGenericException(HttpContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "InternalServerError",
            Title = "Internal Server Error",
            Detail = "An error occurred while processing your request"
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static async Task HandleValidationException(HttpContext context, ValidationException exception)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "ValidationFailure",
            Title = "Validation error",
            Detail = "One or more validation errors has occurred"
        };

        if (exception.Errors.Length != 0)
        {
            problemDetails.Extensions["errors"] = exception.Errors;
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}