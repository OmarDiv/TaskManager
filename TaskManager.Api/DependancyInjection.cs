using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
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
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManager API", Version = "v1" });
                
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token to authorize."
                };

                options.AddSecurityDefinition("Bearer", securityScheme);
                
                options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    { new OpenApiSecuritySchemeReference("Bearer"), new System.Collections.Generic.List<string>() }
                });
            });
            services.AddEndpointsApiExplorer();
            services.AddProblemDetails();

            return services;
        }
    }
}
