using MediatR;
using MoviesApi.Data;
using MoviesApi.Features.Actors.Models;

namespace MoviesApi.Features.Actors;

public record GetActorDetailsQueryRequest(Guid Id) : IRequest<ActorDetailsViewModel?>;

public class GetActorDetailsQueryHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<GetActorDetailsQueryRequest, ActorDetailsViewModel?>
{
    public async Task<ActorDetailsViewModel?> Handle(GetActorDetailsQueryRequest queryRequest,
        CancellationToken cancellationToken)
    {
        var actor = await applicationDbContext.Actors.FindAsync([queryRequest.Id], cancellationToken);
        return actor is null ? null : ActorDetailsViewModel.FromActor(actor);
    }
}