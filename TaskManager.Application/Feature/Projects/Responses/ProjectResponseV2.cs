namespace TaskManager.Application.Feature.Projects.Responses
{
    // V2 Response with breaking changes (e.g., renamed fields or new structure)
    public record ProjectResponseV2(
        long ProjectId,         // Renamed from Id
        string ProjectName,     // Renamed from Name
        string? Summary,        // Renamed from Description
        int TotalTasksCount,    // New Field
        long OwnerId,           // Renamed from CreatedById
        DateTime CreatedAt
    );
}
