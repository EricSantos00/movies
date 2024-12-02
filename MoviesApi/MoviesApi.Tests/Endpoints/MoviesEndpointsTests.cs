using System.Net.Http.Json;
using FluentAssertions;
using MoviesApi.Entities;
using MoviesApi.Features.Movies.Models;

namespace MoviesApi.Tests.Endpoints;

public class MoviesEndpointsTests
{
    [Fact]
    public async Task GetMovies_ReturnsMovies()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var movies = new List<Movie>
        {
            new()
            {
                Title = "movie-title-1",
                Description = "movie-details-1",
                ReleaseDate = DateTime.Now,
                Ratings = [new MovieRating(4)]
            },
            new()
            {
                Title = "movie-title-2",
                Description = "movie-details-2",
                ReleaseDate = DateTime.Now,
                Ratings = [new MovieRating(3)]
            }
        };

        await applicationDbContext.Movies.AddRangeAsync(movies);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var actorsResult = await client.GetFromJsonAsync<List<MovieViewModel>>("/movies");

        actorsResult.Should().NotBeNullOrEmpty();
        actorsResult.Should().HaveCount(2);

        var firstMovie = actorsResult!.First(x => x.Id == movies[0].Id);
        firstMovie.Title.Should().Be(movies[0].Title);
        firstMovie.Description.Should().Be(movies[0].Description);
        firstMovie.ReleaseDate.Should().Be(movies[0].ReleaseDate);
        firstMovie.AverageRating.Should().Be(4);

        var secondMovie = actorsResult!.First(x => x.Id == movies[1].Id);
        secondMovie.Title.Should().Be(movies[1].Title);
        secondMovie.Description.Should().Be(movies[1].Description);
        secondMovie.ReleaseDate.Should().Be(movies[1].ReleaseDate);
        secondMovie.AverageRating.Should().Be(3);
    }

    [Fact]
    public async Task GetMovies_WithFilter_ReturnsFilteredMovies()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var movies = new List<Movie>
        {
            new()
            {
                Title = "The Godfather I",
                Description = "movie-details-1",
                ReleaseDate = DateTime.Now,
                Ratings = [new MovieRating(4)]
            },
            new()
            {
                Title = "The Godfather II",
                Description = "movie-details-2",
                ReleaseDate = DateTime.Now,
                Ratings = [new MovieRating(5)]
            },
            new()
            {
                Title = "The Godfellas",
                Description = "movie-details-3",
                ReleaseDate = DateTime.Now,
                Ratings = [new MovieRating(4)]
            }
        };

        await applicationDbContext.Movies.AddRangeAsync(movies);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var actorsResult = await client.GetFromJsonAsync<List<MovieViewModel>>("/movies?title=Godfa");

        actorsResult.Should().NotBeNullOrEmpty();
        actorsResult.Should().HaveCount(2);
        actorsResult.Should().AllSatisfy(x => { x.Title.Should().Contain("Godfather"); });
    }
}