using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MoviesApi.Entities;
using MoviesApi.Features.Actors;
using MoviesApi.Features.Actors.Models;

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
        var actorsResult = await client.GetFromJsonAsync<List<ActorsViewModel>>("/actors");
        
        actorsResult.Should().NotBeNullOrEmpty();
        actorsResult.Should().HaveCount(2);

        actorsResult![0].Name.Should().Be(actors[0].Name);
        actorsResult[0].Id.Should().Be(actors[0].Id);

        actorsResult[1].Name.Should().Be(actors[1].Name);
        actorsResult[1].Id.Should().Be(actors[1].Id);
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
        var actorsResult = await client.GetFromJsonAsync<List<ActorsViewModel>>("/actors?name=Alexandr");
        
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
            Name = "actor-details",
            Movies = new HashSet<Movie>
            {
                new()
                {
                    Title = "movie-details",
                    Description = "movie-details",
                    ReleaseDate = DateTime.Now,
                    Ratings = [new MovieRating(4)]
                }
            }
        };
        await applicationDbContext.Actors.AddRangeAsync(actor);
        await applicationDbContext.SaveChangesAsync();

        var client = application.CreateClient();
        var actorResult = await client.GetFromJsonAsync<ActorsDetailsViewModel>($"/actors/{actor.Id}");
        
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
            Title = "movie-details",
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
        var actorDetailsResponse = await response.Content.ReadFromJsonAsync<ActorsDetailsViewModel>();
        actorDetailsResponse.Should().NotBeNull();
        actorDetailsResponse!.Name.Should().Be(actorName);
        actorDetailsResponse!.Movies.Should().HaveCount(1);
        actorDetailsResponse.Movies.First().Title.Should().Be(movie.Title);
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
}