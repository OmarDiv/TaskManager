using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor _httpContextAccessor) : IdentityDbContext<ApplicationUser, ApplicationRole, long>(options), IApplicationDbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            var cascade = modelBuilder.Model.GetEntityTypes()
                  .SelectMany(t => t.GetForeignKeys())
                  .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);
            foreach (var fk in cascade)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }
        
        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    var entries = ChangeTracker.Entries<AuditableEntity>();
        //    foreach (var entityEntry in entries)
        //    {
        //        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);
        //        long? currentUserId = userIdStr is null ? null : long.Parse(userIdStr);
        //        if (entityEntry.State == EntityState.Added)
        //        {
        //            entityEntry.Property(e => e.CreatedById).CurrentValue = currentUserId.GetValueOrDefault();
        //        }
        //        else if (entityEntry.State == EntityState.Modified)
        //        {
        //            entityEntry.Property(e => e.UpdatedById).CurrentValue = currentUserId;
        //            entityEntry.Property(e => e.UpdatedOn).CurrentValue = DateTime.UtcNow;
        //        }
        //    }
        //    return base.SaveChangesAsync(cancellationToken);
        //}
    }

}
