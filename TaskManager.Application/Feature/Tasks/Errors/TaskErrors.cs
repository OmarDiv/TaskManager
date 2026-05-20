using Microsoft.AspNetCore.Http;
using TaskManager.Application.Common.Types;

namespace TaskManager.Application.Feature.Tasks.Errors
{
    public static class TaskErrors
    {
        public static readonly Error NotFound = new("Task.NotFound", "The specified task was not found.", StatusCodes.Status404NotFound);
        public static readonly Error UnauthorizedAccess = new("Task.UnauthorizedAccess", "You do not have permission to access or modify this task.", StatusCodes.Status403Forbidden);
        public static readonly Error InvalidStatus = new("Task.InvalidStatus", "The specified task status is invalid.", StatusCodes.Status400BadRequest);
        public static readonly Error InvalidPriority = new("Task.InvalidPriority", "The specified task priority is invalid.", StatusCodes.Status400BadRequest);
    }
}
