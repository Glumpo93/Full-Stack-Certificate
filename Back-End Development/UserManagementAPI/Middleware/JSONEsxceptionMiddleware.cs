using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class JsonExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JsonExceptionMiddleware> _logger;

    public JsonExceptionMiddleware(RequestDelegate next, ILogger<JsonExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception processing request {Method} {Path}", context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started, cannot write JSON error body for {Path}", context.Request.Path);
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new { error = "Internal server error." };
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(payload, options);
            await context.Response.WriteAsync(json);
        }
    }
}