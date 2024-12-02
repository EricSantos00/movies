using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Extensions;
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

        routes.MapGet("/{id:guid}", GetMovieDetails)
            .WithTags("movies")
            .WithDescription("Get a movie details by a given id")
            .WithName(nameof(GetMovieDetails));

        routes.MapPost("/", CreateMovie)
            .WithTags("movies")
            .WithDescription("Create a new movie")
            .WithName(nameof(CreateMovie))
            .RequiresApiKey();

        routes.MapDelete("/{id:guid}", DeleteMovie)
            .WithTags("movies")
            .WithDescription("Deletes a movie")
            .WithName(nameof(DeleteMovie))
            .RequiresApiKey();

        routes.MapPut("/{id:guid}", UpdateMovie)
            .WithTags("movies")
            .WithDescription("Updates a movie")
            .WithName(nameof(UpdateMovie))
            .RequiresApiKey();

        return routes;
    }

    private static async Task<Results<Ok<MovieDetailsViewModel>, NotFound<string>>> UpdateMovie(
        [FromServices] IMediator mediator,
        Guid id,
        UpdateMovieCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        var updateResult = await mediator.Send(request with { Id = id }, cancellationToken);
        if (updateResult)
        {
            return TypedResults.Ok(await mediator.Send(new GetMovieDetailsQueryRequest(id), cancellationToken));
        }

        return TypedResults.NotFound("Actor not found");
    }

    private static async Task<Results<NoContent, NotFound>> DeleteMovie([FromServices] IMediator mediator, Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleteResult = await mediator.Send(new DeleteMovieCommandRequest(id), cancellationToken);
        return deleteResult ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<CreatedAtRoute<MovieDetailsViewModel>> CreateMovie([FromServices] IMediator mediator,
        CreateMovieCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        var createdMovieId = await mediator.Send(request, cancellationToken);
        var movieDetails = await mediator.Send(new GetMovieDetailsQueryRequest(createdMovieId), cancellationToken);
        return TypedResults.CreatedAtRoute(movieDetails, nameof(GetMovieDetails), new { id = createdMovieId });
    }

    private static async Task<Results<Ok<MovieDetailsViewModel>, NotFound>> GetMovieDetails(
        [FromServices] IMediator mediator, Guid id,
        CancellationToken cancellationToken = default)
    {
        var movieDetails = await mediator.Send(new GetMovieDetailsQueryRequest(id), cancellationToken);
        if (movieDetails is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(movieDetails);
    }

    private static async Task<Ok<List<MovieViewModel>>> GetMovies([FromServices] IMediator mediator,
        [AsParameters] GetMoviesQueryRequest request, CancellationToken cancellationToken = default)
    {
        var movies = await mediator.Send(request, cancellationToken);
        return TypedResults.Ok(movies);
    }
}