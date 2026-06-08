using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TaskManager.Api.Filters.Swagger;

public class LanguageHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];

        // Only add Accept-Language header if not already present
        if (!operation.Parameters.Any(p => 
            string.Equals(p.Name, "Accept-Language", StringComparison.OrdinalIgnoreCase)))
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Localization language (supports: ar, en)",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("ar"),
                    // Put ar first as it's the default
                    Enum = 
                    [
                        new OpenApiString("ar"), 
                        new OpenApiString("en")
                    ]
                }
            });
        }
    }
}
