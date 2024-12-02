using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Features.Movies.Models;

namespace MoviesApi.Features.Movies;

public static class MoviesHandlers
{
    public static RouteGroupBuilder MapMovies(this RouteGroupBuilder routes)
    {
        routes.MapGet("/", GetMovies)
            .WithTags("movies")
            .WithDescription("Get all movies")
            .WithName(nameof(GetMovies));

        return routes;
    }

    private static async Task<Ok<List<MovieViewModel>>> GetMovies([FromServices] IMediator mediator,
        [AsParameters] GetMoviesQueryRequest request, CancellationToken cancellationToken = default)
    {
        var movies = await mediator.Send(request, cancellationToken);
        return TypedResults.Ok(movies);
    }
}