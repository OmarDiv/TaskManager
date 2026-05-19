namespace TaskManager.Application.Feature.Roles.Responses
{
    public record RoleResponse(
        long Id,
        string Name,
        bool IsDeleted
        );
}
