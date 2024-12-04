using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MoviesApi.Tests;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContextOptions<TContext>(this IServiceCollection services,
        Action<DbContextOptionsBuilder<TContext>> configure) where TContext : DbContext
    {
        services.RemoveAll<DbContextOptions<TContext>>();

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TContext>();
        configure(dbContextOptionsBuilder);
        services.AddSingleton(dbContextOptionsBuilder.Options);

        services.AddSingleton<DbContextOptions>(s => s.GetRequiredService<DbContextOptions<TContext>>());

        return services;
    }
}