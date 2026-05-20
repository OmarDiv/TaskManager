using System;

namespace TaskManager.Application.Feature.Projects.Responses
{
    public record ProjectResponse(
        long Id,
        string Name,
        string Description,
        DateTime CreatedAt,
        long CreatedById
    );
}
