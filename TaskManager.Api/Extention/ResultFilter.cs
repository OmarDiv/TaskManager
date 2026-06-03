using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using TaskManager.Application.Common.Types;

namespace TaskManager.Api.Extention;

public class ResultFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value is Result result)
            context.HttpContext.Response.StatusCode = (int)result.Type;
        await next();
    }
}
