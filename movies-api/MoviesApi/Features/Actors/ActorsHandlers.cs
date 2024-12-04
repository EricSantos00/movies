using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Extensions;
using MoviesApi.Features.Actors.Models;

namespace MoviesApi.Features.Actors;

public static class ActorsHandlers
{
    public static RouteGroupBuilder MapActors(this RouteGroupBuilder routes)
    {
        routes.MapGet("/", GetActors)
            .WithTags("actors")
            .WithDescription("Get a list of actors")
            .WithName(nameof(GetActors));

        routes.MapGet("/{id:guid}", GetActorDetails)
            .WithTags("actors")
            .WithDescription("Get actor details")
            .WithName(nameof(GetActorDetails));

        routes.MapPost("/", CreateActor)
            .WithTags("actors")
            .WithDescription("Creates a new actor")
            .WithName(nameof(CreateActor))
            .RequiresApiKey();

        routes.MapPut("/{id:guid}", UpdateActor)
            .WithTags("actors")
            .WithDescription("Updates an actor")
            .WithName(nameof(UpdateActor))
            .RequiresApiKey();

        routes.MapDelete("/{id:guid}", DeleteActor)
            .WithTags("actors")
            .WithDescription("Deletes an actor")
            .WithName(nameof(DeleteActor))
            .RequiresApiKey();

        return routes;
    }

    /// <summary>
    /// Deletes an actor using the provided id.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="id">The unique identifier of the actor</param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">Actor was deleted</response>
    /// <response code="404">Actor not found</response>
    private static async Task<Results<NoContent, NotFound>> DeleteActor([FromServices] IMediator mediator, Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleteResult = await mediator.Send(new DeleteActorCommandRequest(id), cancellationToken);
        return deleteResult ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    /// <summary>
    /// Updates an actor with the provided details.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="id">The unique identifier of the actor</param>
    /// <param name="request">The data with the details to be updated</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the actor's updated details</response>
    /// <response code="404">Actor not found</response>
    private static async Task<Results<Ok<ActorDetailsViewModel>, NotFound<string>>> UpdateActor(
        [FromServices] IMediator mediator,
        Guid id,
        UpdateActorCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        var updateResult = await mediator.Send(request with { Id = id }, cancellationToken);
        if (updateResult)
        {
            return TypedResults.Ok(await mediator.Send(new GetActorDetailsQueryRequest(id), cancellationToken));
        }

        return TypedResults.NotFound("Actor not found");
    }

    /// <summary>
    /// Returns a list of actors. If a name is specified, returns the actors that match the name
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="request">The object with the filters to be applied.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the list of actors filtered based on the request</response>
    private static async Task<Ok<List<ActorViewModel>>> GetActors([FromServices] IMediator mediator,
        [AsParameters] GetActorsQueryRequest request, CancellationToken cancellationToken = default)
    {
        var actors = await mediator.Send(request, cancellationToken);
        return TypedResults.Ok(actors);
    }

    /// <summary>
    /// Returns the details of an actor based on the provided id.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="id">The unique identifier of the actor</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the actor's details</response>
    /// <response code="404">Actor not found</response>
    private static async Task<Results<Ok<ActorDetailsViewModel>, NotFound>> GetActorDetails(
        [FromServices] IMediator mediator, Guid id, CancellationToken cancellationToken = default)
    {
        var actorDetails = await mediator.Send(new GetActorDetailsQueryRequest(id), cancellationToken);
        if (actorDetails is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(actorDetails);
    }

    /// <summary>
    /// Creates a new actor with the provided details.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="request">The actor's details to be created</param>
    /// <param name="cancellationToken"></param>
    /// <response code="201">Returns the actor's details for the created actor</response>
    private static async Task<CreatedAtRoute<ActorDetailsViewModel>> CreateActor([FromServices] IMediator mediator,
        CreateActorCommandRequest request, CancellationToken cancellationToken = default)
    {
        var createdActorId = await mediator.Send(request, cancellationToken);
        var actorDetails = await mediator.Send(new GetActorDetailsQueryRequest(createdActorId), cancellationToken);
        return TypedResults.CreatedAtRoute(actorDetails, nameof(GetActorDetails), new { id = createdActorId });
    }
}