using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Authentication;
using TaskManager.Application.Common.Types;
using TaskManager.Application.Feature.Roles.Commands.AddRole;
using TaskManager.Application.Feature.Roles.Responses;
using TaskManager.Application.Feature.Roles.Services;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence.Context;

namespace TaskManager.Infrastructure.Services.Role
{
    public class RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbContext applicationDbContext) : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly ApplicationDbContext _context = applicationDbContext;

        public async Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool? IncludeDisabled = false, CancellationToken cancellation = default) =>
            await _roleManager.Roles.AsNoTracking().Where(r => !r.IsDefault && (!r.IsDeleted || IncludeDisabled.HasValue && IncludeDisabled.Value))
            .ProjectToType<RoleResponse>()
                .ToListAsync(cancellation);


        public async Task<Result<RoleDetailResponse>> GetAsync(long roleId)
        {

            if (await _roleManager.FindByIdAsync(roleId.ToString()) is not { } role)
                return ResultMessage.RoleNotFound;
            var permissions = await _roleManager.GetClaimsAsync(role);
            var response = new RoleDetailResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(clam => clam.Value));

            return Result.Success(response);
        }

        public async Task<Result<RoleDetailResponse>> AddRoleAsync(string name, IList<string> permissions)
        {
            var roleExists = await _roleManager.RoleExistsAsync(name);
            if (roleExists)
                return ResultMessage.RoleAlreadyExists;
            var allowedpermissions = Permissions.GetAllPermissions();

            if (permissions.Except(allowedpermissions).Any())
                return ResultMessage.InvalidPermissions;

            var role = new ApplicationRole
            {
                Name = name,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                var roleClaims = permissions
                    .Select(x => new IdentityRoleClaim<long>
                    {
                        RoleId = role.Id,
                        ClaimType = Permissions.Type,
                        ClaimValue = x
                    }
                    );
                await _context.RoleClaims.AddRangeAsync(roleClaims);
                await _context.SaveChangesAsync();
                var response = new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, permissions);
                return Result.Success(response);
            }

            var errors = result.Errors.First();
            return new ResultMessage(errors.Code, [errors.Description]);
        }

        public async Task<Result> UpdateRoleAsync(long id, string name, IList<string> permissions, CancellationToken cancellation = default)
        {
            if (await _roleManager.FindByIdAsync(id.ToString()) is not { } role)
                return ResultMessage.RoleNotFound;

            var roleExists = await _roleManager.Roles.AnyAsync(r => r.Name == name && r.Id != id);
            if (roleExists)
                return ResultMessage.RoleAlreadyExists;

            var allowedpermissions = Permissions.GetAllPermissions();

            if (permissions.Except(allowedpermissions).Any())
                return ResultMessage.InvalidPermissions;

            role.Name = name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var currentpermissions = await _context.RoleClaims
                    .Where(r => r.RoleId == role.Id && r.ClaimType == Permissions.Type)
                    .Select(r => r.ClaimValue)
                    .ToListAsync(cancellation);

                var newPermissionsToAdd = permissions.Except(currentpermissions).Select(x => new IdentityRoleClaim<long>
                {
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                });

                var permissionsToRemove = currentpermissions.Except(permissions);
                var claimsToRemove = await _context.RoleClaims
                    .Where(r => r.RoleId == role.Id && r.ClaimType == Permissions.Type && permissionsToRemove.Contains(r.ClaimValue))
                    .ToListAsync(cancellation);

                _context.RoleClaims.RemoveRange(claimsToRemove);
                await _context.RoleClaims.AddRangeAsync(newPermissionsToAdd, cancellation);
                await _context.SaveChangesAsync(cancellation);

                return Result.Success();
            }

            var errors = result.Errors.First();
            return new ResultMessage(errors.Code, [errors.Description]);
        }

        public async Task<Result> ToggleStatusAsync(long id)
        {
            if (await _roleManager.FindByIdAsync(id.ToString()) is not { } role)
                return ResultMessage.RoleNotFound;
            role.IsDeleted = !role.IsDeleted;
            await _roleManager.UpdateAsync(role);
            return Result.Success();
        }

    }
}
