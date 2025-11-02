using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenValidationMiddleware> _logger;
    private readonly IConfiguration _config;

    public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger, IConfiguration config)
    {
        _next = next;
        _logger = logger;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // PUBLIC PATHS / SKIP LIST
        // Allow anonymous access to the dev auth endpoint, swagger UI and static assets, health checks, and OPTIONS (CORS preflight).
        var path = context.Request.Path;
        if (path.StartsWithSegments("/auth", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(context.Request.Method, HttpMethods.Options, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Expect Authorization: Bearer <token>
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
        {
            await ReturnUnauthorized(context, "Authorization header missing.");
            return;
        }

        var authHeader = authHeaderValues.ToString();
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            await ReturnUnauthorized(context, "Invalid authorization scheme.");
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token))
        {
            await ReturnUnauthorized(context, "Token is missing.");
            return;
        }

        try
        {
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                _logger.LogError("JWT configuration is missing (Jwt:Key/Issuer/Audience).");
                await ReturnUnauthorized(context, "Token validation configuration is incomplete.");
                return;
            }

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            context.User = principal;
            await _next(context);
        }
        catch (SecurityTokenException ste)
        {
            _logger.LogWarning(ste, "Token validation failed for request {Path}", context.Request.Path);
            await ReturnUnauthorized(context, "Invalid or expired token.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating token for request {Path}", context.Request.Path);
            await ReturnUnauthorized(context, "Invalid token.");
        }
    }

    private static async Task ReturnUnauthorized(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        context.Response.Headers["WWW-Authenticate"] = "Bearer";

        var payload = new { error = "Unauthorized", message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}