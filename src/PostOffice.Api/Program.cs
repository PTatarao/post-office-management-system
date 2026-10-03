using Microsoft.AspNetCore.Diagnostics;
using PostOffice.Application.Common.Exceptions;
using PostOffice.Application.Services;
using PostOffice.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Built-in OpenAPI
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<PostOfficeService>();
builder.Services.AddScoped<ShipmentService>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});
var app = builder.Build();

app.UseExceptionHandler();

app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

// Redirect root to OpenAPI JSON
app.MapGet("/", () => Results.Redirect("/openapi/v1.json"));

app.Run();

public partial class Program
{
}

sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            NotFoundException =>
                (StatusCodes.Status404NotFound, "Resource not found"),

            ArgumentException =>
                (StatusCodes.Status400BadRequest, "Invalid request"),

            InvalidOperationException =>
                (StatusCodes.Status409Conflict, "Business rule violation"),

            _ =>
                (StatusCodes.Status500InternalServerError, "Unexpected error")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        httpContext.Response.StatusCode = status;

        await Results.Problem(
            statusCode: status,
            title: title,
            detail: status == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : exception.Message,
            instance: httpContext.Request.Path
        ).ExecuteAsync(httpContext);

        return true;
    }
}