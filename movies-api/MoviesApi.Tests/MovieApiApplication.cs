using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesApi.Data;
using MoviesApi.Tests.Handlers;

namespace MoviesApi.Tests;

internal class MovieApiApplication : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _sqliteConnection = new("Filename=:memory:");

    public ApplicationDbContext CreateApplicationDbContext()
    {
        var db = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext();

        // Drop and recreate the database to remove all the fake data
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        return db;
    }

    public HttpClient CreateAuthorizedClient()
    {
        return CreateDefaultClient(new ApiKeyHandler(Services));
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        _sqliteConnection.Open();

        builder.ConfigureServices(services =>
        {
            services.AddDbContextFactory<ApplicationDbContext>();
            services.AddDbContextOptions<ApplicationDbContext>(o => o.UseSqlite(_sqliteConnection));
        });

        return base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        _sqliteConnection.Dispose();
        base.Dispose(disposing);
    }
}