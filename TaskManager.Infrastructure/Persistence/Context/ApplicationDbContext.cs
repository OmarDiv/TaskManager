using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Entities.Common;

namespace TaskManager.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor _httpContextAccessor) : IdentityDbContext<ApplicationUser, ApplicationRole, long>(options), IApplicationDbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> Tasks { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<LocalizationSet> LocalizationSets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var cascade = modelBuilder.Model.GetEntityTypes()
                  .SelectMany(t => t.GetForeignKeys())
                  .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership && fk.DeclaringEntityType.ClrType != typeof(ProjectTask));
            foreach (var fk in cascade)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }
    }
}

