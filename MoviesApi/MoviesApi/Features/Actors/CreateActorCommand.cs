using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Entities;

namespace MoviesApi.Features.Actors;

public record CreateActorCommandRequest(string Name, List<Guid>? Movies)
    : IRequest<Guid>;

public class CreateActorCommandRequestValidator : AbstractValidator<CreateActorCommandRequest>
{
    public CreateActorCommandRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class CreateActorCommandHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateActorCommandRequest, Guid>
{
    public async Task<Guid> Handle(CreateActorCommandRequest request, CancellationToken cancellationToken)
    {
        var actor = new Actor
        {
            Name = request.Name,
        };

        if (request.Movies is not null && request.Movies.Count != 0)
        {
            var existingMovies = await applicationDbContext.Movies
                .Where(x => request.Movies.Contains(x.Id))
                .ToHashSetAsync(cancellationToken);

            actor.Movies = existingMovies;
        }

        await applicationDbContext.Actors.AddAsync(actor, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return actor.Id;
    }
}