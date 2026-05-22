using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Roles.Responses;
using TaskManager.Application.Feature.Roles.Services;

namespace TaskManager.Application.Feature.Roles.Queries.GetAllRoles
{
    public record GetAllRoles(bool? IncludeDisabled) : IRequest<Result<IEnumerable<RoleResponse>>>;
    
    public class GetAllRolesHandler(IRoleService _roleService) : IRequestHandler<GetAllRoles, Result<IEnumerable<RoleResponse>>>
    {
        public async Task<Result<IEnumerable<RoleResponse>>> Handle(GetAllRoles request, CancellationToken cancellationToken)
        {
            var roles = await _roleService.GetAllRolesAsync(request.IncludeDisabled, cancellationToken);
            return Result.Success(roles);
        }
    }
}
