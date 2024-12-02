using System.Reflection;
using Microsoft.OpenApi.Models;
using MoviesApi.Authentication;

namespace MoviesApi.Extensions;

public static class SwaggerConfigurationExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        var scheme = new OpenApiSecurityScheme
        {
            Description = "The API key to access the API. Default value: 'api-key'",
            In = ParameterLocation.Header,
            Name = AuthenticationConstants.ApiKeyHeaderName,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKeyScheme",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "ApiKey"
            }
        };

        services.AddSwaggerGen(x =>
        {
            x.AddSecurityDefinition("ApiKey", scheme);
            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { scheme, new List<string>() }
            });

            x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });

        return services;
    }
}