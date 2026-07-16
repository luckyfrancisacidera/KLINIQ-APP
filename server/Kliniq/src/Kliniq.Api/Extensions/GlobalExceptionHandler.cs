using FluentValidation;
using Kliniq.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kliniq.Api.Extensions
{
    public sealed class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, title) = MapException(exception);

            if(statusCode == StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);

            httpContext.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = GetProblemType(statusCode),
                Instance = httpContext.Request.Path,
                Detail = GetSafeDetail(exception)
            };

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

            if(exception is ValidationException ve)
            {
                problemDetails.Extensions["errors"] = ve.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            }

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });

        }

        private static (int statusCode, string title) MapException(Exception exception) => exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation Failed"),

            DomainException => (StatusCodes.Status422UnprocessableEntity, "Business Rule Violation"),

            DbUpdateException => (StatusCodes.Status409Conflict, "Data Conflict"),

            InvalidOperationException => (StatusCodes.Status409Conflict, "Operation Not Allowed"),

            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),

            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),

            ArgumentNullException or ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),

            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")

        };

        private static string GetProblemType(int statusCode) => statusCode switch
        {
            400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            422 => "https://tools.ietf.org/html/rfc9110#section-15.5.21",
            _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };
        
        private static string GetSafeDetail(Exception exception)
            => exception is DomainException or ValidationException
                ? exception.Message
                : "The request could not be completed. Use the traceId to investigate.";
    }
}
