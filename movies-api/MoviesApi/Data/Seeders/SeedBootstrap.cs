using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Data.Seeders;

public static class SeedBootstrap
{
    public static async Task SeedDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await applicationDbContext.Database.MigrateAsync();

        await MovieActorSeeder.SeedAsync(applicationDbContext);
    }
}