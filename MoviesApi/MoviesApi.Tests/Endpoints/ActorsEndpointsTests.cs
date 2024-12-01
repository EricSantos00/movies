using System.Net.Http.Json;
using FluentAssertions;
using MoviesApi.Entities;
using MoviesApi.Features.Actors.Models;

namespace MoviesApi.Tests.Endpoints;

public class ActorsEndpointsTests
{
    [Fact]
    public async Task GetActors()
    {
        await using var application = new MovieApiApplication();
        await using var applicationDbContext = application.CreateApplicationDbContext();

        var actors = new List<Actor>
        {
            new()
            {
                Name = "actor-1"
            },
            new()
            {
                Name = "actor-2"
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
}