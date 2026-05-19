using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.Feature.Roles.Responses
{
    public record RoleDetailResponse(
        long Id,
        string Name,
        bool IsDeleted,
        IEnumerable<string> Permissions
        );
}
