using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PostOffice.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception while processing request {Method} {Path}", context.Request.Method, context.Request.Path);

                if (context.Response.HasStarted)
                {
                   throw;
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Default: 500 Internal Server Error
            var status = StatusCodes.Status500InternalServerError;
            var title = "An unexpected error occurred.";
            var detail = exception.Message;

            // Minimal, runtime-only mapping by exception type name to avoid compile-time dependency on domain exception types.
            // If your project exposes specific exception types, replace these string checks with 'is' pattern matches.
            var exName = exception.GetType().Name;
            switch (exName)
            {
                case "NotFoundException":
                    status = StatusCodes.Status404NotFound;
                    title = "Not Found";
                    break;
                case "BadRequestException":
                case "ValidationException":
                    status = StatusCodes.Status400BadRequest;
                    title = "Bad Request";
                    break;
                case "UnauthorizedException":
                case "AuthenticationException":
                    status = StatusCodes.Status401Unauthorized;
                    title = "Unauthorized";
                    break;
                case "ForbiddenException":
                    status = StatusCodes.Status403Forbidden;
                    title = "Forbidden";
                    break;
            }

            var problem = new ProblemDetails
            {
                Type = "about:blank",
                Title = title,
                Status = status,
                Detail = detail,
                Instance = context.Request.Path
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problem, options);

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            return context.Response.WriteAsync(json);
        }
    }

   }
