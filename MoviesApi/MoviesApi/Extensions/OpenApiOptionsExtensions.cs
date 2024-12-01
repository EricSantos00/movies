using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using MoviesApi.Authentication;

namespace MoviesApi.Extensions;

public static class OpenApiOptionsExtensions
{
    public static OpenApiOptions AddApiKeyAuthentication(this OpenApiOptions options)
    {
        const string schemeIdentifier = "ApiKey";

        var scheme = new OpenApiSecurityScheme
        {
            Description = "The API key to access the API. Use hard-coded value 'api-key'.",
            Name = AuthenticationConstants.ApiKeyHeaderName,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKeyScheme",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = schemeIdentifier
            },
        };

        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes.Add(schemeIdentifier, scheme);
            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, _) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.Contains(typeof(ApiKeyAuthenticationMiddleware)))
            {
                operation.Security = [new OpenApiSecurityRequirement { [scheme] = [] }];
            }

            return Task.CompletedTask;
        });

        return options;
    }
}