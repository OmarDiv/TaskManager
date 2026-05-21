using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.Application.Common.Authentication;
using TaskManager.Application.Common.Interfaces.Persistence;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Application.Common.Interfaces.Services;
using TaskManager.Application.Feature.Auth.Services;
using TaskManager.Application.Feature.Roles.Services;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence.Context;
using TaskManager.Infrastructure.Services.Auth;
using TaskManager.Infrastructure.Services.Caching;
using TaskManager.Infrastructure.Services.Role;

namespace TaskManager.Infrastructure
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.
                DbContextConfig(configuration)
                .AuthConfig(configuration);

            var redisConnection = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnection))
            {
                services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "_TaskManager";
            });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }


            services.AddScoped<IApplicationDbContext>(provider =>
                           provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped(typeof(IGenericRepository<>), typeof(Persistence.Repositories.GenericRepository<>));
            services.AddScoped<IUnitOfWork, Persistence.Repositories.UnitOfWork>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddSingleton<ICacheService, CacheService>();

            return services;
        }
        private static IServiceCollection DbContextConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(connectionString, sqlOptions =>
                 {
                     sqlOptions.EnableRetryOnFailure(
                         maxRetryCount: 5,
                         maxRetryDelay: TimeSpan.FromSeconds(30),
                         errorNumbersToAdd: null);
                 }));

            return services;
        }


        private static IServiceCollection AuthConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();
            var setting = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting!.Key))
                };
            });
            services.Configure<IdentityOptions>(
                options =>
                {
                    options.Password.RequiredLength = 8;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.User.RequireUniqueEmail = true;
                }
                );
            services.AddSingleton<IJwtProvider, JwtProvider>();

            return services;
        }
    }
}
