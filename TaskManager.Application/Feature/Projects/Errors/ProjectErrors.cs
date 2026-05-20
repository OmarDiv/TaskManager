using Microsoft.AspNetCore.Http;
using TaskManager.Application.Common.Types;

namespace TaskManager.Application.Feature.Projects.Errors
{
    public static class ProjectErrors
    {
        public static readonly Error NotFound = new("Project.NotFound", "The specified project was not found.", StatusCodes.Status404NotFound);
        public static readonly Error UnauthorizedAccess = new("Project.UnauthorizedAccess", "You do not have permission to access or modify this project.", StatusCodes.Status403Forbidden);
        public static readonly Error DuplicateName = new("Project.DuplicateName", "A project with this name already exists for your account.", StatusCodes.Status409Conflict);
    }
}
