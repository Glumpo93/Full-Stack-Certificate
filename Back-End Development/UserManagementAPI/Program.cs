using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Management API", Version = "v1" });
});

// Register application services/repositories
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1"));
}
else
{
    // Global exception handler for non-development environments
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError
            };

            // In Development we may include details; here we keep it minimal for production
            if (app.Environment.IsDevelopment() && feature?.Error != null)
                problem.Detail = feature.Error.ToString();

            await JsonSerializer.SerializeAsync(context.Response.Body, problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        });
    });
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<JsonExceptionMiddleware>();

app.UseMiddleware<TokenValidationMiddleware>();

app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<SimpleRequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();