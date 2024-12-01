using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesApi.Data;
using MoviesApi.Tests.Handlers;

namespace MoviesApi.Tests;

internal class MovieApiApplication : WebApplicationFactory<Program>
{
    public ApplicationDbContext CreateApplicationDbContext()
    {
        var db = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext();

        // Drop and recreate the database to remove all the fake data
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        return db;
    }

    public HttpClient CreateAuthenticatedClient()
    {
        return CreateDefaultClient(new ApiKeyHandler(Services));
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddDbContextFactory<ApplicationDbContext>();
            services.AddDbContextOptions<ApplicationDbContext>(o => o.UseInMemoryDatabase("MoviesDb"));
        });

        return base.CreateHost(builder);
    }
}