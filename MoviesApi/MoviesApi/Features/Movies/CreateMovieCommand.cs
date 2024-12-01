using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Entities;

namespace MoviesApi.Features.Movies;

public record CreateMovieCommandRequest(string Title, string Description, DateTime ReleaseDate, List<Guid>? Actors)
    : IRequest<Guid>;

public class CreateMovieCommandRequestValidator : AbstractValidator<CreateMovieCommandRequest>
{
    public CreateMovieCommandRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}

public class CreateMovieCommandHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateMovieCommandRequest, Guid>
{
    public async Task<Guid> Handle(CreateMovieCommandRequest request, CancellationToken cancellationToken)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            ReleaseDate = request.ReleaseDate
        };

        if (request.Actors is not null && request.Actors.Count != 0)
        {
            var existingActors = await applicationDbContext.Actors
                .Where(x => request.Actors.Contains(x.Id))
                .ToHashSetAsync(cancellationToken);

            movie.Actors = existingActors;
        }

        await applicationDbContext.Movies.AddAsync(movie, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return movie.Id;
    }
}