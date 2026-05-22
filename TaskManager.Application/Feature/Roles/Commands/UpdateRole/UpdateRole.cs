using TaskManager.Application.Feature.Roles.Services;

namespace TaskManager.Application.Feature.Roles.Commands.UpdateRole
{
    public record UpdateRole(long id, string Name, IList<string> Permissions) : IRequest<Result>;
    
    public class UpdateRoleHandler(IRoleService _roleService) : IRequestHandler<UpdateRole, Result>
    {
        public async Task<Result> Handle(UpdateRole request, CancellationToken cancellationToken)
        {
            var result = await _roleService.UpdateRoleAsync(request.id, request.Name, request.Permissions, cancellationToken);
            return result;
        }
    }
}
