using TaskManager.Application.Feature.Roles.Responses;
using TaskManager.Application.Feature.Roles.Services;

namespace TaskManager.Application.Feature.Roles.Queries.GetRoleById
{
    public record GetRoleById(long Id) : IRequest<Result<RoleDetailResponse>>;

    public class GetRoleByIdHandler(IRoleService _roleService) : IRequestHandler<GetRoleById, Result<RoleDetailResponse>>
    {
        public async Task<Result<RoleDetailResponse>> Handle(GetRoleById request, CancellationToken cancellationToken)
        {
            return await _roleService.GetAsync(request.Id);
        }
    }
}
