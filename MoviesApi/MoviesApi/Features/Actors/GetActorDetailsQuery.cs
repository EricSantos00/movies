using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Features.Actors.Models;

namespace MoviesApi.Features.Actors;

/// <summary>
/// Represents a query to fetch detailed information about a specific actor based on their unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the actor whose details are being requested.</param>
public record GetActorDetailsQueryRequest(Guid Id) : IRequest<ActorDetailsViewModel?>;

public class GetActorDetailsQueryHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<GetActorDetailsQueryRequest, ActorDetailsViewModel?>
{
    public async Task<ActorDetailsViewModel?> Handle(GetActorDetailsQueryRequest queryRequest,
        CancellationToken cancellationToken)
    {
        var actor = await applicationDbContext.Actors
            .AsNoTracking()
            .Include(x => x.Movies)
            .FirstOrDefaultAsync(x => x.Id == queryRequest.Id, cancellationToken);
        
        return actor is null ? null : ActorDetailsViewModel.FromActor(actor);
    }
}