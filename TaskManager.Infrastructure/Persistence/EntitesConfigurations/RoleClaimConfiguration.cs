using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using TaskManager.Application.Common.Authentication;
using TaskManager.Infrastructure.Persistence.Data;

namespace TaskManager.Infrastructure.Persistence.EntitesConfigurations
{
    public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<long>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<long>> builder)
        {
            var permissions = Permissions.GetAllPermissions();
            var roleClaims = permissions
                .Where(p => p != null)
                .Select((permission, index) => new IdentityRoleClaim<long>
                {
                    Id = index + 1,
                    RoleId = DefaultRoles.Admin.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = permission!
                })
                .ToArray();

            builder.HasData(roleClaims);
        }
    }
}
