using System;
using System.Collections.Generic;
using TaskManager.Application.Common.Localizations;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Feature.Tasks.Responses
{
    public record TaskResponse(
        long Id,
        List<LocalizationDto> Titles,
        string? Title,
        List<LocalizationDto> Descriptions,
        string? Description,
        string Status,
        DateTime? DueDate,
        string Priority,
        long ProjectId
    );
}
