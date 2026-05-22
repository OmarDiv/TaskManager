using TaskManager.Application.Feature.Roles.Responses;
using TaskManager.Application.Feature.Roles.Services;
namespace TaskManager.Application.Feature.Roles.Commands.AddRole
{
    public record AddRole(string Name, IList<string> Permissions) : IRequest<Result<RoleDetailResponse>>;

    public class AddRoleHandler(IRoleService _roleService) : IRequestHandler<AddRole, Result<RoleDetailResponse>>
    {
        public async Task<Result<RoleDetailResponse>> Handle(AddRole request, CancellationToken cancellationToken)
        {
            var result = await _roleService.AddRoleAsync(request.Name, request.Permissions);
            return result;
        }
    }
}
