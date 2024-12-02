using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;

namespace MoviesApi.Features.Actors;

/// <summary>
/// Represents a command with the data required to update an actor's details
/// </summary>
/// <param name="Id">The unique identifier of the actor to be updated.</param>
/// <param name="Name">The updated name of the actor.</param>
/// <param name="Movies">An optional list of unique identifiers for the movies associated with the actor.</param>
public record UpdateActorCommandRequest(
   [property: JsonIgnore] Guid Id, string Name, List<Guid>? Movies) : IRequest<bool>;

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