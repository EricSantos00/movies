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

        routes.MapPost("/{id:guid}/rate", RateMovie)
            .WithTags("movies")
            .WithDescription("Rates a movie")
            .WithName(nameof(RateMovie))
            .RequiresApiKey();

        return routes;
    }

    /// <summary>
    /// Rates a specific movie with the rate value
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="id">The unique identifier of the movie</param>
    /// <param name="request">The data containing the rating details</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the movie updated details</response>
    /// <response code="404">Movie not found</response>
    private static async Task<Results<Ok<MovieDetailsViewModel>, NotFound<string>>> RateMovie(
        [FromServices] IMediator mediator,
        Guid id,
        RateMovieCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        var rateResult = await mediator.Send(request with { Id = id }, cancellationToken);
        if (rateResult)
        {
            return TypedResults.Ok(await mediator.Send(new GetMovieDetailsQueryRequest(id), cancellationToken));
        }

        return TypedResults.NotFound("Movie not found");
    }

    /// <summary>
    /// Updates a movie with the provided details.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="id">The unique identifier of the movie</param>
    /// <param name="request">The data with the details to be updated</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the movie updated details</response>
    /// <response code="404">Movie not found</response>
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

    /// <summary>
    /// Deletes a movie using the provided id.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="id">The unique identifier of the movie</param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">Movie was deleted</response>
    /// <response code="404">Movie not found</response>
    private static async Task<Results<NoContent, NotFound>> DeleteMovie([FromServices] IMediator mediator, Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleteResult = await mediator.Send(new DeleteMovieCommandRequest(id), cancellationToken);
        return deleteResult ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    /// <summary>
    /// Creates a new movie with the provided details.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="request">The movie details to be created</param>
    /// <param name="cancellationToken"></param>
    /// <response code="201">Returns the movie details for the created movie</response>
    private static async Task<CreatedAtRoute<MovieDetailsViewModel>> CreateMovie([FromServices] IMediator mediator,
        CreateMovieCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        var createdMovieId = await mediator.Send(request, cancellationToken);
        var movieDetails = await mediator.Send(new GetMovieDetailsQueryRequest(createdMovieId), cancellationToken);
        return TypedResults.CreatedAtRoute(movieDetails, nameof(GetMovieDetails), new { id = createdMovieId });
    }

    /// <summary>
    /// Returns the details of a movie based on the provided id.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="id">The unique identifier of the movie</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the movie details</response>
    /// <response code="404">Movie not found</response>
    private static async Task<Results<Ok<MovieDetailsViewModel>, NotFound>> GetMovieDetails(
        [FromServices] IMediator mediator, Guid id,
        CancellationToken cancellationToken = default)
    {
        var movieDetails = await mediator.Send(new GetMovieDetailsQueryRequest(id), cancellationToken);
        if (movieDetails is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(movieDetails);
    }

    /// <summary>
    /// Returns a list of movies. If a title is specified, returns the movies that match the title
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="request">The object with the filters to be applied.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the list of movies filtered based on the request</response>
    private static async Task<Ok<List<MovieViewModel>>> GetMovies([FromServices] IMediator mediator,
        [AsParameters] GetMoviesQueryRequest request, CancellationToken cancellationToken = default)
    {
        var movies = await mediator.Send(request, cancellationToken);
        return TypedResults.Ok(movies);
    }
}