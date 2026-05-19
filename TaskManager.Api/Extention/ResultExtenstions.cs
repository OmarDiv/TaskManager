using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Types;

namespace TaskManager.Api.Extention
{

    public static class ResultExtensions
    {
        public static ActionResult<T> AsActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return result.Value is null ? new NotFoundResult() : new OkObjectResult(result.Value);

            return CreateProblemResult(result);
        }

        public static ActionResult AsNoContentResult(this Result result)
        {
            return result.IsSuccess
                ? new NoContentResult()
                : CreateProblemResult(result);
        }

        public static ActionResult<T> AsCreatedResult<T>(this Result<T> result, string routeName, object routeValues)
        {
            return result.IsSuccess
                ? new CreatedAtRouteResult(routeName, routeValues, result.Value)
                : CreateProblemResult(result);
        }
        private static ObjectResult CreateProblemResult(Result result)
        {

            var problemDetails = new
            {
                Title = GetTitle(result.Error.StatusCode),
                Status = result.Error.StatusCode,
                Errors = new[] { new { result.Error.Code, result.Error.Description } }

            };

            return new ObjectResult(problemDetails) { StatusCode = result.Error.StatusCode };
        }

        private static string GetTitle(int? statusCode) => statusCode switch
        {
            400 => "Bad Request",
            404 => "Not Found",
            409 => "Conflict",
            401 => "Unauthorized",
            403 => "Forbidden",
            500 => "Internal Server Error",
            _ => "Error"
        };
    }

}
