using System;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.DTOs.Tasks
{
    public record CreateTaskDto(
        string Title,
        string Description,
        Status Status,
        DateTime? DueDate,
        Priority Priority,
        long ProjectId
    );
}
