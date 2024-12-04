using Bogus;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Entities;

namespace MoviesApi.Data.Seeders;

public static class MovieActorSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
        if (await dbContext.Actors.AnyAsync() && await dbContext.Movies.AnyAsync())
            return;
        
        var actorsGenerator = new Faker<Actor>()
            .RuleFor(a => a.Name, f => f.Name.FullName());
        var actors = actorsGenerator.Generate(100);
        await dbContext.Actors.AddRangeAsync(actors);
        await dbContext.SaveChangesAsync();

        var movieRatingGenerator = new Faker<MovieRating>()
            .CustomInstantiator(f => new MovieRating(f.Random.Number(0, 5)));

        var moviesGenerator = new Faker<Movie>()
            .RuleFor(m => m.Title, f => f.Random.Words(3))
            .RuleFor(m => m.Description, f => f.Lorem.Sentence(100))
            .RuleFor(m => m.ReleaseDate, f => f.Date.Past())
            .RuleFor(m => m.Actors, f => f.PickRandom(actors, 20).ToHashSet());

        var movies = moviesGenerator.Generate(100);
        foreach (var movie in movies)
        {
            movie.Ratings = movieRatingGenerator.Generate(20).ToList();
        }

        await dbContext.Movies.AddRangeAsync(movies);
        await dbContext.SaveChangesAsync();
    }
}