using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        var requestBody = await ReadRequestBodyAsync(context);
        var headers = context.Request.Headers
            .Where(h => !string.Equals(h.Key, "Authorization", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        _logger.LogInformation("Incoming Request {Method} {Path} Headers={Headers} Body={Body}",
            context.Request.Method, context.Request.Path, headers, requestBody);

        var originalBody = context.Response.Body;
        await using var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        try
        {
            await _next(context);
        }
        finally
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body, Encoding.UTF8).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation("Outgoing Response {StatusCode} Body={Body}", context.Response.StatusCode, responseText);

            await responseStream.CopyToAsync(originalBody);
            context.Response.Body = originalBody;
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpContext context)
    {
        try
        {
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            return string.IsNullOrWhiteSpace(body) ? "<empty>" : body;
        }
        catch
        {
            return "<unreadable>";
        }
    }
}