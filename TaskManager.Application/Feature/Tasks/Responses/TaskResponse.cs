using System;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Feature.Tasks.Responses
{
    public record TaskResponse(
        long Id,
        string Title,
        string Description,
        string Status,
        DateTime? DueDate,
        string Priority,
        long ProjectId
    );
}
