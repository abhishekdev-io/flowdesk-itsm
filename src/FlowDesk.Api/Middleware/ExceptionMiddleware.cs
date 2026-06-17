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
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

        object response = new 
        {
            error = "Internal Server Error"
        };

        switch (exception)
        {
            //FluentValidation 
            case ValidationException validationException:

                statusCode = HttpStatusCode.BadRequest;
                response = new
                {
                    error = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                };
                break;

            //Domain 
            case DomainStateException stateException:

                statusCode = HttpStatusCode.Conflict;
                response = new
                {
                     error = "Invalid State Transition", 
                     details = stateException.Message
             };
                break;
                
            case DomainException domainException:

                statusCode = HttpStatusCode.BadRequest;
                response = new
                {
                    error = "Domain Error",
                    details = domainException.Message
                };
                break;   

            //Not Found
            case NotFoundException notFoundException:

                statusCode = HttpStatusCode.NotFound;
                response = new
                {
                    error = "Not Found",
                    details = notFoundException.Message
                };
                break;

            //Conflict
            case ConflictException conflictException:

                statusCode = HttpStatusCode.Conflict;
                response = new
                {
                    error = "Conflict",
                    details = conflictException.Message
                };
                break; 

            //Permision
            case PermissionMethodNotFoundException permissionException:

                _logger.LogError(permissionException,
                    "Permission method not found. This indicates a missing handler in PermissionService.");

                statusCode = HttpStatusCode.InternalServerError;
                response = new
                {
                    error = "Permission method notfound",
                    details = permissionException.Message
                };
                break;   

            //Forbidden    
            case ForbiddenException forbiddenException:

                statusCode = HttpStatusCode.Forbidden;
                response = new
                {
                    error = "Forbidden",
                    details = forbiddenException.Message
                };
                break;

            //Unauthorized    
            case UnauthorizedAccessException unauthorizedAccessException:

                statusCode = HttpStatusCode.Unauthorized;
                response = new
                {
                    error = "Unauthorized",
                    details = unauthorizedAccessException.Message
                };
                break;

            //Identity
            case IdentityException identityException: 

                statusCode = HttpStatusCode.BadRequest;
                response = new
                {
                    error = "Identity Error",
                    details = identityException.Errors
                };
                break;  

            default:
                _logger.LogError(exception,
                    "Unhandled exception on {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);
                break;    
        }

        context.Response.StatusCode = (int)statusCode;

        return context.Response
        .WriteAsync(JsonSerializer.Serialize(response));
    }
}

