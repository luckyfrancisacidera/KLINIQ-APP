using Kliniq.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Extensions;

public static class ResultExtension
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
        => result.IsSuccess
            ? new OkObjectResult(result.Value)
            : MapError(result.Error!);

    public static IActionResult ToCreatedResult<T>(this Result<T> result, string routeName, object routeValues)
        => result.IsSuccess
            ? new CreatedAtRouteResult(routeName, routeValues, result.Value)
            : MapError(result.Error!);

    public static IActionResult ToActionResult(this Result result)
        => result.IsSuccess
            ? new OkResult()
            : MapError(result.Error!);

    private static IActionResult MapError(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode switch
            {
                StatusCodes.Status400BadRequest => "Invalid request",
                StatusCodes.Status401Unauthorized => "Authentication required",
                StatusCodes.Status404NotFound => "Resource not found",
                StatusCodes.Status409Conflict => "Request conflict",
                _ => "An unexpected error occurred"
            },
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "The request could not be completed."
                : error.Message,
            Type = $"https://httpstatuses.com/{statusCode}"
        };
        problem.Extensions["code"] = error.Code;

        return new ObjectResult(problem)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };
    }
}
