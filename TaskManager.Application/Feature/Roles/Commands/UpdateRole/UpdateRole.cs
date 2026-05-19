using TaskManager.Application.Feature.Roles.Commands.AddRole;
using TaskManager.Application.Feature.Roles.Services;

namespace TaskManager.Application.Feature.Roles.Commands.UpdateRole
{
    public record UpdateRole(long id, RoleRequest Role) : IRequest<Result>;
    
    public class UpdateRoleHandler(IRoleService _roleService) : IRequestHandler<UpdateRole, Result>
    {
        public async Task<Result> Handle(UpdateRole request, CancellationToken cancellationToken)
        {
            var result = await _roleService.UpdateRoleAsync(request.id, request.Role, cancellationToken);
            return result;
        }
    }
}
