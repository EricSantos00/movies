namespace MoviesApi.Authentication;

public class ApiKeyAuthenticationMiddleware : IEndpointFilter
{
    private const string ApiKeyHeaderName = "x-api-key";

    // This value would normally be injected from the constructor.
    private const string ApiKeyHeaderValue = "api-key";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
        {
            return TypedResults.Unauthorized();
        }

        if (!ApiKeyHeaderValue.Equals(apiKey))
        {
            return TypedResults.Unauthorized();
        }

        return await next(context);
    }
}