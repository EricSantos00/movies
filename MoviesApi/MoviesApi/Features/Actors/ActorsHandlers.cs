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
            .WithDescription("Get all actors")
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

    private static async Task<Results<NoContent, NotFound>> DeleteActor([FromServices] IMediator mediator, Guid id)
    {
        var deleteResult = await mediator.Send(new DeleteActorCommandRequest(id));
        return deleteResult ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ActorDetailsViewModel>, NotFound<string>>> UpdateActor(
        [FromServices] IMediator mediator,
        Guid id,
        UpdateActorCommandRequest request)
    {
        var updateResult = await mediator.Send(request with { Id = id });
        if (updateResult)
        {
            return TypedResults.Ok(await mediator.Send(new GetActorDetailsQueryRequest(id)));
        }

        return TypedResults.NotFound("Actor not found");
    }

    private static async Task<Ok<List<ActorViewModel>>> GetActors([FromServices] IMediator mediator,
        [AsParameters] GetActorsQueryRequest request)
    {
        var actors = await mediator.Send(request);
        return TypedResults.Ok(actors);
    }

    private static async Task<Results<Ok<ActorDetailsViewModel>, NotFound>> GetActorDetails(
        [FromServices] IMediator mediator, Guid id)
    {
        var actorDetails = await mediator.Send(new GetActorDetailsQueryRequest(id));
        if (actorDetails is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(actorDetails);
    }

    private static async Task<CreatedAtRoute<ActorDetailsViewModel>> CreateActor([FromServices] IMediator mediator,
        CreateActorCommandRequest request)
    {
        var result = await mediator.Send(request);
        var actorDetails = await mediator.Send(new GetActorDetailsQueryRequest(result));
        return TypedResults.CreatedAtRoute(actorDetails, nameof(GetActorDetails), new { id = result });
    }
}