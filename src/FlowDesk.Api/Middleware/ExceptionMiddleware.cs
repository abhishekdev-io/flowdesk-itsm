using System.Net;
using System.Text.Json;
using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Infrastructure.Identity.Exceptions;
using FlowDesk.Domain.Exceptions;
using FluentValidation;

namespace FlowDesk.Api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    // Reuse serializer options to prevent allocation overhead on every error response
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Single point of logging for all exceptions bubbling out of use-cases
            _logger.LogError(ex, "Unhandled exception occurred during request {Method} {Path}", 
                context.Request.Method, 
                context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, body) = Map(exception);

        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(body, SerializerOptions));
    }

    private static (HttpStatusCode StatusCode, ErrorResponse Body) Map(Exception exception)
    {
        return exception switch
        {
            ValidationException ex => (
                HttpStatusCode.BadRequest, 
                new ErrorResponse(
                    "ValidationError",
                    "One or more validation errors occurred.",
                    ex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => JsonNamingPolicy.CamelCase.ConvertName(g.Key), // Formats dictionary keys to camelCase
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        )
                )),

            DomainStateException ex => (
                HttpStatusCode.Conflict, 
                new ErrorResponse("InvalidStateTransition", ex.Message, null)),

            DomainException ex => (
                HttpStatusCode.BadRequest, 
                new ErrorResponse("DomainError", ex.Message, null)),

            NotFoundException ex => (
                HttpStatusCode.NotFound, 
                new ErrorResponse("NotFound", ex.Message, null)),

            ConflictException ex => (
                HttpStatusCode.Conflict, 
                new ErrorResponse("Conflict", ex.Message, null)),

            ForbiddenException ex => (
                HttpStatusCode.Forbidden, 
                new ErrorResponse("Forbidden", ex.Message, null)),

            UnauthorizedAccessException ex => (
                HttpStatusCode.Unauthorized, 
                new ErrorResponse("Unauthorized", ex.Message, null)),

            IdentityException ex => (
                HttpStatusCode.BadRequest, 
                new ErrorResponse("IdentityError", "Identity operation failed.", ex.Errors)),

            PermissionMethodNotFoundException ex => (
                HttpStatusCode.InternalServerError, 
                new ErrorResponse("InternalError", "A security configuration error occurred.", null)),

            _ => (
                HttpStatusCode.InternalServerError, 
                new ErrorResponse("InternalError", "An unexpected error occurred.", null))
        };
    }
}

public sealed record ErrorResponse(string Error, string Message, object? Details);