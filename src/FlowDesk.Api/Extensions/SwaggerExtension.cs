using Microsoft.OpenApi;

namespace FlowDesk.Api.Extensions;

public static class SwaggerExtension
{
    public static void AddSwaggerDocumentation(this IServiceCollection services)
    {

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo  
            {
                Title = "FlowDesk API",
                Version = "v1"
            });
            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });
        });
    }
}


