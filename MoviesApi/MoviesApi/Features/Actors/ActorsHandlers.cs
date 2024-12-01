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
        routes.MapGet("/{id:guid}", GetActorDetails)
            .WithTags("actors")
            .WithDescription("Get actor details")
            .WithName(nameof(GetActorDetails))
            .RequiresApiKey();

        routes.MapPost("/", CreateActor)
            .WithTags("actors")
            .WithDescription("Creates a new actor")
            .WithName(nameof(CreateActor))
            .RequiresApiKey();

        return routes;
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