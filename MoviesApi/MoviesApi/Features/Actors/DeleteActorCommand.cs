using MediatR;
using MoviesApi.Data;

namespace MoviesApi.Features.Actors;

public record DeleteActorCommandRequest(Guid Id) : IRequest<bool>;

public class DeleteActorCommandHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<DeleteActorCommandRequest, bool>
{
    public async Task<bool> Handle(DeleteActorCommandRequest request, CancellationToken cancellationToken)
    {
        var actor = await applicationDbContext.Actors.FindAsync([request.Id], cancellationToken);
        if (actor is null)
        {
            return false;
        }

        applicationDbContext.Actors.Remove(actor);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}