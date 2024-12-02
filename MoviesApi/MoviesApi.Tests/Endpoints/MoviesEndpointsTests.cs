using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Entities;
using MoviesApi.Features.Movies;
using MoviesApi.Features.Movies.Models;
using MoviesApi.Validation;

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
        var moviesResult = await client.GetFromJsonAsync<List<MovieViewModel>>("/movies");

        moviesResult.Should().NotBeNullOrEmpty();
        moviesResult.Should().HaveCount(2);

        var firstMovie = moviesResult!.First(x => x.Id == movies[0].Id);
        firstMovie.Title.Should().Be(movies[0].Title);
        firstMovie.Description.Should().Be(movies[0].Description);
        firstMovie.ReleaseDate.Should().Be(movies[0].ReleaseDate);
        firstMovie.AverageRating.Should().Be(4);

        var secondMovie = moviesResult!.First(x => x.Id == movies[1].Id);
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
        var moviesResult = await client.GetFromJsonAsync<List<MovieViewModel>>("/movies?title=Godfa");

        moviesResult.Should().NotBeNullOrEmpty();
        moviesResult.Should().HaveCount(2);
        moviesResult.Should().AllSatisfy(x => { x.Title.Should().Contain("Godfather"); });
    }

    [Fact]
    public async Task GetMovieDetails_NonExistingMovie_ReturnsNotFound()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var client = application.CreateClient();
        var result = await client.GetAsync($"/movies/{Guid.NewGuid()}");

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMovieDetails_ExistingMovie_ReturnsMovieDetails()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var movie = new Movie
        {
            Title = "The Godfather I",
            Description = "movie-details-1",
            ReleaseDate = DateTime.Now,
            Ratings = [new MovieRating(5)],
            Actors = new HashSet<Actor>
            {
                new()
                {
                    Name = "Al Pacino"
                }
            }
        };
        await applicationDbContext.Movies.AddRangeAsync(movie);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var movieDetailsResult = await client.GetFromJsonAsync<MovieDetailsViewModel>($"/movies/{movie.Id}");

        movieDetailsResult.Should().NotBeNull();
        movieDetailsResult!.Id.Should().Be(movie.Id);
        movieDetailsResult.Title.Should().Be(movie.Title);
        movieDetailsResult.Description.Should().Be(movie.Description);
        movieDetailsResult.ReleaseDate.Should().Be(movie.ReleaseDate);
        movieDetailsResult.AverageRating.Should().Be(5);
        movieDetailsResult.Actors.Should().HaveCount(1);
        movieDetailsResult.Actors.First().Name.Should().Be("Al Pacino");
    }

    [Fact]
    public async Task CreateMovie_ReturnsCreatedMovieDetails()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var actor = new Actor
        {
            Name = "Al Pacino"
        };
        await applicationDbContext.Actors.AddAsync(actor);
        await applicationDbContext.SaveChangesAsync();
        var createMovieCommandRequest =
            new CreateMovieCommandRequest("The Godfather II", "movie-description", DateTime.Now, [actor.Id, actor.Id]);

        var client = application.CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("/movies", createMovieCommandRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var movieDetailsResponse = await response.Content.ReadFromJsonAsync<MovieDetailsViewModel>();
        movieDetailsResponse.Should().NotBeNull();
        movieDetailsResponse!.Title.Should().Be(createMovieCommandRequest.Title);
        movieDetailsResponse.Description.Should().Be(createMovieCommandRequest.Description);
        movieDetailsResponse.ReleaseDate.Should().Be(createMovieCommandRequest.ReleaseDate);
        movieDetailsResponse.AverageRating.Should().Be(0);
        movieDetailsResponse.Actors.Should().HaveCount(1);
        movieDetailsResponse.Actors.First().Name.Should().Be("Al Pacino");
    }

    [Fact]
    public async Task CreateMovie_InvalidFieldLength_ReturnsBadRequest()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var actor = new Actor
        {
            Name = "Al Pacino"
        };
        await applicationDbContext.Actors.AddAsync(actor);
        await applicationDbContext.SaveChangesAsync();
        var createMovieCommandRequest =
            new CreateMovieCommandRequest(new string('a', 501), new string('a', 2001), DateTime.Now, [actor.Id]);

        var client = application.CreateAuthorizedClient();
        var response =
            await client.PostAsJsonAsync("/movies", createMovieCommandRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails!.Type.Should().Be("ValidationFailure");
        problemDetails.Title.Should().Be("Validation error");
        problemDetails.Detail.Should().Be("One or more validation errors has occurred");

        var errors = JsonSerializer.Deserialize<ValidationError[]>(problemDetails.Extensions["errors"]!.ToString()!,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        errors!.Length.Should().Be(2);
        errors.Should().Contain(x => x.PropertyName == "Title" && x.ErrorMessage ==
            "The length of 'Title' must be 500 characters or fewer. You entered 501 characters.");
        errors.Should().Contain(x => x.PropertyName == "Description" && x.ErrorMessage ==
            "The length of 'Description' must be 2000 characters or fewer. You entered 2001 characters.");
    }

    [Fact]
    public async Task CreateMovie_UnauthorizedClient_ReturnsForbidden()
    {
        await using var application = new MovieApiApplication();

        var client = application.CreateClient();
        var response =
            await client.PostAsJsonAsync("/movies",
                new CreateMovieCommandRequest("The Godfather II", "movie-description", DateTime.Now, []));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMovie_UnauthorizedClient_ReturnsForbidden()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var movie = new Movie
        {
            Title = "The Godfather I",
            Description = "movie-details-1",
            ReleaseDate = DateTime.Now,
            Ratings = [new MovieRating(5)]
        };
        await applicationDbContext.Movies.AddRangeAsync(movie);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var response = await client.DeleteAsync($"/movies/{movie.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMovie_NonExistingMovie_ReturnsNotFound()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var client = application.CreateAuthorizedClient();
        var response = await client.DeleteAsync($"/movies/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteMovie_DeletesMovie()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var actor = new Actor
        {
            Name = "actor-name"
        };
        var movie = new Movie
        {
            Title = "movie-title",
            Description = "movie-details",
            ReleaseDate = DateTime.Now,
            Ratings = [new MovieRating(4)],
            Actors = new HashSet<Actor>
            {
                actor
            }
        };
        await applicationDbContext.Movies.AddRangeAsync(movie);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateAuthorizedClient();
        var response = await client.DeleteAsync($"/movies/{movie.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var movieDetailsResponse = await client.GetAsync($"/movies/{movie.Id}");
        movieDetailsResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var actorInDb = await applicationDbContext.Actors.FindAsync(actor.Id);
        actorInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateMovie_UnauthorizedClient_ReturnsForbidden()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var movie = new Movie
        {
            Title = "The Godfather I",
            Description = "movie-details-1",
            ReleaseDate = DateTime.Now,
            Ratings = [new MovieRating(5)]
        };
        await applicationDbContext.Movies.AddRangeAsync(movie);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var response = await client.PutAsJsonAsync($"/movies/{movie.Id}", movie);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMovie_NonExistingMovie_ReturnsNotFound()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var movie = new Movie
        {
            Title = "The Godfather I",
            Description = "movie-details-1",
            ReleaseDate = DateTime.Now,
            Ratings = [new MovieRating(5)]
        };

        var client = application.CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync($"/movies/{Guid.NewGuid()}", movie);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateMovie_UpdatesMovie()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var movie = new Movie
        {
            Title = "movie-title",
            Description = "movie-details",
            ReleaseDate = DateTime.Now,
            Ratings = [new MovieRating(4)],
            Actors = new HashSet<Actor>
            {
                new()
                {
                    Name = "actor-name-1"
                },
                new()
                {
                    Name = "actor-name-2"
                },
            }
        };

        await applicationDbContext.Movies.AddAsync(movie);
        await applicationDbContext.SaveChangesAsync();
        var updateMovieCommandRequest = new UpdateMovieCommandRequest(movie.Id, "updated-title", "updated-description",
            DateTime.Now, [movie.Actors.First().Id]);

        var client = application.CreateAuthorizedClient();
        var movieUpdateResult =
            await client.PutAsJsonAsync($"/movies/{updateMovieCommandRequest.Id}", updateMovieCommandRequest);

        movieUpdateResult.Should().NotBeNull();
        movieUpdateResult.StatusCode.Should().Be(HttpStatusCode.OK);
        var movieDetailsResponse = await movieUpdateResult.Content.ReadFromJsonAsync<MovieDetailsViewModel>();

        movieDetailsResponse!.Id.Should().Be(movie.Id);
        movieDetailsResponse.Title.Should().Be(updateMovieCommandRequest.Title);
        movieDetailsResponse.Description.Should().Be(updateMovieCommandRequest.Description);
        movieDetailsResponse.ReleaseDate.Should().Be(updateMovieCommandRequest.ReleaseDate);
        movieDetailsResponse.AverageRating.Should().Be(4);
        movieDetailsResponse.Actors.Should().HaveCount(1);
        movieDetailsResponse.Actors.First().Name.Should().Be("actor-name-1");
    }

    [Fact]
    public async Task UpdateMovie_InvalidFieldsLength_ReturnsBadRequest()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var movie = new Movie
        {
            Title = "movie-title",
            Description = "movie-details",
            ReleaseDate = DateTime.Now,
            Ratings = [new MovieRating(4)],
            Actors = new HashSet<Actor>
            {
                new()
                {
                    Name = "actor-name-1"
                },
                new()
                {
                    Name = "actor-name-2"
                },
            }
        };
        await applicationDbContext.Movies.AddAsync(movie);
        await applicationDbContext.SaveChangesAsync();
        var updateMovieCommandRequest =
            new UpdateMovieCommandRequest(movie.Id, new string('a', 501), new string('a', 2001), DateTime.Now, []);

        var client = application.CreateAuthorizedClient();
        var result =
            await client.PutAsJsonAsync($"/movies/{updateMovieCommandRequest.Id}", updateMovieCommandRequest);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails!.Type.Should().Be("ValidationFailure");
        problemDetails.Title.Should().Be("Validation error");
        problemDetails.Detail.Should().Be("One or more validation errors has occurred");

        var errors = JsonSerializer.Deserialize<ValidationError[]>(problemDetails.Extensions["errors"]!.ToString()!,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        errors!.Length.Should().Be(2);
        errors.Should().Contain(x => x.PropertyName == "Title" && x.ErrorMessage ==
            "The length of 'Title' must be 500 characters or fewer. You entered 501 characters.");
        errors.Should().Contain(x => x.PropertyName == "Description" && x.ErrorMessage ==
            "The length of 'Description' must be 2000 characters or fewer. You entered 2001 characters.");
    }
}