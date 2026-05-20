using System.Collections.Generic;
using System.Linq;

namespace TaskManager.Application.Common.Authentication
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";

        // ======================= Users =======================
        public const string GetUsers = "users:read";
        public const string AddUser = "users:add";
        public const string UpdateUser = "users:update";
        public const string DeleteUser = "users:delete";
        public const string ToggleUserActive = "users:toggle";

        // ======================= Roles =======================
        public const string GetRoles = "roles:read";
        public const string AddRole = "roles:add";
        public const string UpdateRole = "roles:update";
        public const string DeleteRole = "roles:delete";
        public const string ManageRolePermissions = "roles:permissions";

        // ======================= Projects =======================
        public const string GetProjects = "projects:read";
        public const string AddProject = "projects:add";
        public const string UpdateProject = "projects:update";
        public const string DeleteProject = "projects:delete";

        // ======================= Tasks =======================
        public const string GetTasks = "tasks:read";
        public const string AddTask = "tasks:add";
        public const string UpdateTask = "tasks:update";
        public const string DeleteTask = "tasks:delete";

        public static IList<string?> GetAllPermissions() =>
            typeof(Permissions)
                .GetFields()
                .Where(f => f.IsLiteral && !f.IsInitOnly)
                .Select(x => x.GetValue(x) as string)
                .ToList();
    }
}