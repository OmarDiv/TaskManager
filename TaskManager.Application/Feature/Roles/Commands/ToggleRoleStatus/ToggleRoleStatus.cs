using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.Feature.Roles.Services;

namespace TaskManager.Application.Feature.Roles.Commands.ToggleRoleStatus
{
    public  record ToggleRoleStatus(long RoleId) : IRequest<Result>;
    public class ToggleRoleStatusHandler(IRoleService _roleService) : IRequestHandler<ToggleRoleStatus, Result>
    {
        public async Task<Result> Handle(ToggleRoleStatus request, CancellationToken cancellationToken)
        {
            var result = await _roleService.ToggleStatusAsync(request.RoleId);
            return result;
        }
    }
}
