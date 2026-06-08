using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;
using TaskManager.Api;
using TaskManager.Api.Middlewares;
using TaskManager.Application;
using TaskManager.Application.Common.Types;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddPresentation(builder.Configuration);

var app = builder.Build();

// UseRequestLocalization FIRST!
ConfigureLocalization(app);


// Automatic Migration for Docker environment
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable Swagger in Production for evaluation purposes
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }
});
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

void ConfigureLocalization(WebApplication app)
{
    var arCulture = new CultureInfo("ar")
    {
        NumberFormat =
        {
            DigitSubstitution = DigitShapes.NativeNational,
            NumberDecimalSeparator = "."
        },
        DateTimeFormat =
        {
            AMDesignator = "AM",
            PMDesignator = "PM"
        }
    };

    var supportedCultures = new[] { arCulture, new CultureInfo("en") };

    var options = new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("ar"),
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures
    };

    // Only use Accept-Language header - ignore query string and cookies!
    options.RequestCultureProviders.Clear();
    options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

    app.UseRequestLocalization(options);
}