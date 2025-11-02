using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class SimpleRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SimpleRequestLoggingMiddleware> _logger;

    public SimpleRequestLoggingMiddleware(RequestDelegate next, ILogger<SimpleRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;

        try
        {
            await _next(context);
        }
        finally
        {
            var statusCode = context.Response?.StatusCode ?? 0;
            _logger.LogInformation("{Method} {Path} responded {StatusCode}", method, path, statusCode);
        }
    }
}