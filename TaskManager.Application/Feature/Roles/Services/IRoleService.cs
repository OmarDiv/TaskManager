using TaskManager.Application.Feature.Roles.Commands.AddRole;
using TaskManager.Application.Feature.Roles.Responses;

namespace TaskManager.Application.Feature.Roles.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool? IncludeDisabled = false, CancellationToken cancellationToken = default);
        Task<Result<RoleDetailResponse>> GetAsync(long roleId);
        Task<Result<RoleDetailResponse>> AddRoleAsync(RoleRequest request);
        Task<Result> UpdateRoleAsync(long id, RoleRequest request, CancellationToken cancellationToken);
        Task<Result> ToggleStatusAsync(long id);
    }
}
