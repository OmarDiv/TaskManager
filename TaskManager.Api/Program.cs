using Microsoft.EntityFrameworkCore;
using TaskManager.Api;
using TaskManager.Application;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddPresentation(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

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

app.UseExceptionHandler();

// Enable Swagger in Production for evaluation purposes
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
