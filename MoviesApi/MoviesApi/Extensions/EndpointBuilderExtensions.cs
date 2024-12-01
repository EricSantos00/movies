using MoviesApi.Authentication;

namespace MoviesApi.Extensions;

public static class EndpointBuilderExtensions
{
    public static RouteHandlerBuilder RequiresApiKey(this RouteHandlerBuilder builder)
    {
        builder
            .WithMetadata(typeof(ApiKeyAuthenticationMiddleware))
            .AddEndpointFilter<ApiKeyAuthenticationMiddleware>();

        return builder;
    }
}