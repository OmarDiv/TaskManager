using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using TaskManager.Application.Common.Authentication;
using TaskManager.Application.Common.Localizations;

namespace TaskManager.Application
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DependancyInjection).Assembly;
            
            // Mapster configurations
            var mappingConfig = TypeAdapterConfig.GlobalSettings;
            mappingConfig.Scan(assembly);

            services.AddSingleton<IStringLocalizerFactory, JsonStringLcalizerFactory>();
            services.AddSingleton<IStringLocalizer, JsonStringLocalizer>();
            services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}