using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Sehaty_Plus.Infrastructure.Persistence.Data;
using System.Threading.RateLimiting;
using TaskManager.Api.Errors;
namespace TaskManager.Api
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!));
            });
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddSwaggerGen();
            services.AddEndpointsApiExplorer();
            services.AddProblemDetails();

            return services;
        }
    }
}
