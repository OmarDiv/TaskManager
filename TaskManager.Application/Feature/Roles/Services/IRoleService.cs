using TaskManager.Application.Feature.Roles.Commands.AddRole;
using TaskManager.Application.Feature.Roles.Responses;

namespace TaskManager.Application.Feature.Roles.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool? IncludeDisabled = false, CancellationToken cancellationToken = default);
        Task<Result<RoleDetailResponse>> GetAsync(long roleId);
        Task<Result<RoleDetailResponse>> AddRoleAsync(string name, IList<string> permissions);
        Task<Result> UpdateRoleAsync(long id, string name, IList<string> permissions, CancellationToken cancellationToken);
        Task<Result> ToggleStatusAsync(long id);
    }
}
