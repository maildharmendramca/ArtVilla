using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace IndianArtVilla.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedDomainException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access");
            await WriteResponseAsync(context, HttpStatusCode.Unauthorized, ex.Message, ex);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            await WriteResponseAsync(context, HttpStatusCode.NotFound, ex.Message, ex);
        }
        catch (BusinessRuleViolationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation");
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain exception");
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access");
            await WriteResponseAsync(context, HttpStatusCode.Unauthorized, "Unauthorized.", ex);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            await WriteResponseAsync(context, HttpStatusCode.NotFound, ex.Message, ex);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Bad request");
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError,
                "An unexpected error occurred.", ex);
        }
    }

    private async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var errors = _env.IsDevelopment()
            ? new List<string> { ex.Message, ex.StackTrace ?? string.Empty }
            : null;

        var response = ApiResponse<object>.Fail(message, errors);

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
