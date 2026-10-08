using Microsoft.AspNetCore.Diagnostics;
using PostOffice.Api.Middleware;
using PostOffice.Application.Common.Exceptions;
using PostOffice.Application.Services;
using PostOffice.Infrastructure;
using System.Net;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
// Built-in OpenAPI
builder.Services.AddOpenApi();

builder.Services.AddScoped<PostOfficeService>();
builder.Services.AddScoped<ShipmentService>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyDomain", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AnyDomain");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

app.MapControllers();

// Redirect root to OpenAPI JSON
app.MapGet("/", () => Results.Redirect("/openapi/v1.json"));

app.Run();
