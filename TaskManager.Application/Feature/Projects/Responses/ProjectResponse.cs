using TaskManager.Application.Common.Localizations;

namespace TaskManager.Application.Feature.Projects.Responses
{
    public record ProjectResponse(
     long Id,
     List<LocalizationDto> Names,
     string? Name,
     List<LocalizationDto> Descriptions,
    string? Description,
     DateTime CreatedAt,
     long CreatedById
    );
}
