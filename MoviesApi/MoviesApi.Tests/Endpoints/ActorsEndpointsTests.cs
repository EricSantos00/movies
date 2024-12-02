using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Entities;
using MoviesApi.Features.Actors;
using MoviesApi.Features.Actors.Models;
using MoviesApi.Validation;

namespace MoviesApi.Tests.Endpoints;

public class ActorsEndpointsTests
{
    [Fact]
    public async Task GetActors_ReturnsActors()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var actors = new List<Actor>
        {
            new()
            {
                Name = "Leonardo",
            },
            new()
            {
                Name = "Alexander",
            }
        };

        await applicationDbContext.Actors.AddRangeAsync(actors);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var actorsResult = await client.GetFromJsonAsync<List<ActorViewModel>>("/actors");

        actorsResult.Should().NotBeNullOrEmpty();
        actorsResult.Should().HaveCount(2);

        actorsResult.Should().Contain(x => x.Name == actors[0].Name && x.Id == actors[0].Id);
        actorsResult.Should().Contain(x => x.Name == actors[1].Name && x.Id == actors[1].Id);
    }

    [Fact]
    public async Task GetActors_WithFilter_ReturnsFilteredActors()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var actors = new List<Actor>
        {
            new()
            {
                Name = "Alexandre Luis"
            },
            new()
            {
                Name = "Alexandre Guilherme"
            },
            new()
            {
                Name = "Dominic King"
            }
        };

        await applicationDbContext.Actors.AddRangeAsync(actors);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var actorsResult = await client.GetFromJsonAsync<List<ActorViewModel>>("/actors?name=Alexandr");

        actorsResult.Should().NotBeNullOrEmpty();
        actorsResult.Should().HaveCount(2);
        actorsResult.Should().AllSatisfy(x => { x.Name.Should().Contain("Alexandre"); });
    }

    [Fact]
    public async Task GetActorDetails_NonExistingActor_ReturnsNotFound()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var client = application.CreateClient();
        var result = await client.GetAsync($"/actors/{Guid.NewGuid()}");

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetActorDetails_ExistingActor_ReturnsActorDetails()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var actor = new Actor
        {
            Name = "actor-name",
            Movies = new HashSet<Movie>
            {
                new()
                {
                    Title = "movie-title",
                    Description = "movie-details",
                    ReleaseDate = DateTime.Now,
                    Ratings = [new MovieRating(4)]
                }
            }
        };
        await applicationDbContext.Actors.AddAsync(actor);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var actorResult = await client.GetFromJsonAsync<ActorDetailsViewModel>($"/actors/{actor.Id}");

        actorResult.Should().NotBeNull();
        actorResult!.Id.Should().Be(actor.Id);
        actorResult.Name.Should().Be(actor.Name);
        actorResult.Movies.Should().HaveCount(1);
        actorResult.Movies.First().Title.Should().Be(actor.Movies.First().Title);
        actorResult.Movies.First().Id.Should().Be(actor.Movies.First().Id);
        actorResult.Movies.First().Description.Should().Be(actor.Movies.First().Description);
        actorResult.Movies.First().ReleaseDate.Should().Be(actor.Movies.First().ReleaseDate);
        actorResult.Movies.First().AverageRating.Should().Be(4);
    }

    [Fact]
    public async Task CreateActor_ReturnsCreatedActorDetails()
    {
        const string actorName = "Luis Garcia";
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var movie = new Movie
        {
            Title = "movie-title",
            Description = "movie-details",
            ReleaseDate = DateTime.Now,
            Ratings = [new MovieRating(4)]
        };
        await applicationDbContext.Movies.AddAsync(movie);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateAuthorizedClient();
        var response =
            await client.PostAsJsonAsync("/actors", new CreateActorCommandRequest(actorName, [movie.Id]));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var actorDetailsResponse = await response.Content.ReadFromJsonAsync<ActorDetailsViewModel>();
        actorDetailsResponse.Should().NotBeNull();
        actorDetailsResponse!.Name.Should().Be(actorName);
        actorDetailsResponse!.Movies.Should().HaveCount(1);
        actorDetailsResponse.Movies.First().Title.Should().Be(movie.Title);
    }

    [Fact]
    public async Task CreateActor_InvalidName_ReturnsBadRequest()
    {
        var actorName = new string('a', 101);
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var movie = new Movie
        {
            Title = "movie-title",
            Description = "movie-details",
            ReleaseDate = DateTime.Now,
            Ratings = [new MovieRating(4)]
        };
        await applicationDbContext.Movies.AddAsync(movie);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateAuthorizedClient();
        var response =
            await client.PostAsJsonAsync("/actors", new CreateActorCommandRequest(actorName, [movie.Id]));

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
        errors!.Length.Should().Be(1);
        errors[0].PropertyName.Should().Be("Name");
        errors[0].ErrorMessage.Should()
            .Be("The length of 'Name' must be 100 characters or fewer. You entered 101 characters.");
    }

    [Fact]
    public async Task CreateActor_UnauthorizedClient_ReturnsForbidden()
    {
        await using var application = new MovieApiApplication();

        var client = application.CreateClient();
        var response =
            await client.PostAsJsonAsync("/actors", new CreateActorCommandRequest("Luis Garcia", []));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteActor_UnauthorizedClient_ReturnsForbidden()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var actor = new Actor
        {
            Name = "actor-name"
        };
        await applicationDbContext.Actors.AddAsync(actor);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var response = await client.DeleteAsync($"/actors/{actor.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteActor_NonExistingActor_ReturnsNotFound()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var client = application.CreateAuthorizedClient();
        var response = await client.DeleteAsync($"/actors/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteActor_DeletesActor()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var movie = new Movie
        {
            Title = "movie-title",
            Description = "movie-details",
            ReleaseDate = DateTime.Now,
            Ratings = [new MovieRating(4)]
        };
        var actor = new Actor
        {
            Name = "actor-name",
            Movies = new HashSet<Movie>
            {
                movie
            }
        };
        await applicationDbContext.Actors.AddAsync(actor);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateAuthorizedClient();
        var response = await client.DeleteAsync($"/actors/{actor.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var actorDetails = await client.GetAsync($"/actors/{actor.Id}");
        actorDetails.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var movieInDb = await applicationDbContext.Movies.FindAsync(movie.Id);
        movieInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateActor_UnauthorizedClient_ReturnsForbidden()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var actor = new Actor
        {
            Name = "actor-name"
        };
        await applicationDbContext.Actors.AddAsync(actor);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var response = await client.PutAsJsonAsync($"/actors/{actor.Id}", actor);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateActor_NonExistingActor_ReturnsNotFound()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var actor = new Actor
        {
            Name = "actor-name"
        };

        var client = application.CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync($"/actors/{Guid.NewGuid()}", actor);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateActor_UpdatesActor()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var actor = new Actor
        {
            Name = "actor-name",
            Movies = new HashSet<Movie>
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
                    Ratings = [new MovieRating(4)]
                }
            }
        };
        var updateActorCommandRequest =
            new UpdateActorCommandRequest(actor.Id, "actor-updated", [actor.Movies.First().Id]);

        await applicationDbContext.Actors.AddAsync(actor);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateAuthorizedClient();
        var actorResult =
            await client.PutAsJsonAsync($"/actors/{updateActorCommandRequest.Id}", updateActorCommandRequest);

        actorResult.Should().NotBeNull();
        actorResult.StatusCode.Should().Be(HttpStatusCode.OK);
        var actorDetailsResponse = await actorResult.Content.ReadFromJsonAsync<ActorDetailsViewModel>();

        actorDetailsResponse!.Id.Should().Be(actor.Id);
        actorDetailsResponse.Name.Should().Be(updateActorCommandRequest.Name);
        actorDetailsResponse.Movies.Should().HaveCount(1);
        actorDetailsResponse.Movies.First().Id.Should().Be(updateActorCommandRequest.Movies!.First());
    }

    [Fact]
    public async Task UpdateActor_InvalidName_ReturnsBadRequest()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();
        var actor = new Actor
        {
            Name = "actor-details",
            Movies = new HashSet<Movie>
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
                    Ratings = [new MovieRating(4)]
                }
            }
        };
        var updateActorCommandRequest =
            new UpdateActorCommandRequest(actor.Id, new string('a', 101), [actor.Movies.First().Id]);

        await applicationDbContext.Actors.AddAsync(actor);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateAuthorizedClient();
        var result =
            await client.PutAsJsonAsync($"/actors/{updateActorCommandRequest.Id}", updateActorCommandRequest);

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
        errors!.Length.Should().Be(1);
        errors[0].PropertyName.Should().Be("Name");
        errors[0].ErrorMessage.Should()
            .Be("The length of 'Name' must be 100 characters or fewer. You entered 101 characters.");
    }
}