using Kliniq.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Kliniq.Api.Extensions
{
    public static class ResultExtension
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            return MapError(result.Error!);
        }

        public static IActionResult ToCreatedResult<T>(this Result<T> result, string routeName, object routeValues)
        {
            if(result.IsSuccess)
                return new CreatedAtRouteResult(routeName, routeValues, result.Value);

            return MapError(result.Error!);
        }

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
                return new OkResult();

            return MapError(result.Error!);
        }

        private static IActionResult MapError(Error error) => error.Type switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(ToProblem(error)),
            ErrorType.Validation => new BadRequestObjectResult(ToProblem(error)),
            ErrorType.Conflict => new ConflictObjectResult(ToProblem(error)),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(ToProblem(error)),
            _ => new ObjectResult(ToProblem(error)) { StatusCode = 500 }
        };

        private static Object ToProblem(Error error) => new
        {
            error.Code,
            error.Message,
            error.Type
        };
    }
}
