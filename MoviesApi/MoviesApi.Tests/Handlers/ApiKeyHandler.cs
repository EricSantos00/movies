using Microsoft.Extensions.DependencyInjection;
using MoviesApi.Authentication;

namespace MoviesApi.Tests.Handlers;

internal sealed class ApiKeyHandler(IServiceProvider services) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        request.Headers.Add(AuthenticationConstants.ApiKeyHeaderName, "api-key");
        return await base.SendAsync(request, cancellationToken);
    }
}