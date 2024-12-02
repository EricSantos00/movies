using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;

namespace MoviesApi.Features.Actors;

public record UpdateActorCommandRequest(Guid Id, string Name, List<Guid>? Movies) : IRequest<bool>;

public class UpdateActorCommandRequestValidator : AbstractValidator<UpdateActorCommandRequest>
{
    public UpdateActorCommandRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class UpdateActorCommandHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<UpdateActorCommandRequest, bool>
{
    public async Task<bool> Handle(UpdateActorCommandRequest request, CancellationToken cancellationToken)
    {
        var actor = await applicationDbContext.Actors
            .Include(x => x.Movies)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (actor is null)
        {
            return false;
        }

        actor.Name = request.Name;

        if (request.Movies is not null)
        {
            var existingMovies = await applicationDbContext.Movies
                .Where(x => request.Movies.Contains(x.Id))
                .ToHashSetAsync(cancellationToken);

            actor.Movies = existingMovies;
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}