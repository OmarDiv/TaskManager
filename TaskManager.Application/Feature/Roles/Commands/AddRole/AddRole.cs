using TaskManager.Application.Feature.Roles.Responses;
using TaskManager.Application.Feature.Roles.Services;
namespace TaskManager.Application.Feature.Roles.Commands.AddRole
{
    public record RoleRequest(string Name, IList<string> Permissions);
    public record AddRole(RoleRequest Role) : IRequest<Result<RoleDetailResponse>>;

    public class AddRoleHandler(IRoleService _roleService) : IRequestHandler<AddRole, Result<RoleDetailResponse>>
    {
        public async Task<Result<RoleDetailResponse>> Handle(AddRole request, CancellationToken cancellationToken)
        {
            var result = await _roleService.AddRoleAsync(request.Role);
            return result;
        }
    }
}
