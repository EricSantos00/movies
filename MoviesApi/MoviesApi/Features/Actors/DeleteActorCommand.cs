using MediatR;
using MoviesApi.Data;

namespace MoviesApi.Features.Actors;

/// <summary>
/// Represents a command to delete an actor from the system.
/// </summary>
/// <param name="Id">The unique identifier of the actor to be deleted.</param>
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