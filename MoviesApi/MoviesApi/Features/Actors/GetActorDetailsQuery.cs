using MediatR;
using MoviesApi.Data;
using MoviesApi.Features.Actors.Models;

namespace MoviesApi.Features.Actors;

public record GetActorDetailsQueryRequest(Guid Id) : IRequest<ActorsDetailsViewModel?>;

public class GetActorDetailsQueryHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<GetActorDetailsQueryRequest, ActorsDetailsViewModel?>
{
    public async Task<ActorsDetailsViewModel?> Handle(GetActorDetailsQueryRequest queryRequest,
        CancellationToken cancellationToken)
    {
        var actor = await applicationDbContext.Actors.FindAsync([queryRequest.Id], cancellationToken);
        return actor is null ? null : ActorsDetailsViewModel.FromActor(actor);
    }
}