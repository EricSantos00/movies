using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;

namespace MoviesApi.Features.Movies;

public record UpdateMovieCommandRequest(
    Guid Id,
    string Title,
    string Description,
    DateTime ReleaseDate,
    List<Guid>? Actors) : IRequest<bool>;

public class UpdateMovieCommandRequestValidator : AbstractValidator<UpdateMovieCommandRequest>
{
    public UpdateMovieCommandRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}

public class UpdateMovieCommandHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<UpdateMovieCommandRequest, bool>
{
    public async Task<bool> Handle(UpdateMovieCommandRequest request, CancellationToken cancellationToken)
    {
        var movie = await applicationDbContext.Movies
            .Include(x => x.Actors)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (movie is null)
        {
            return false;
        }

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.ReleaseDate = request.ReleaseDate;

        if (request.Actors is not null)
        {
            var existingActors = await applicationDbContext.Actors
                .Where(x => request.Actors.Contains(x.Id))
                .ToHashSetAsync(cancellationToken);

            movie.Actors = existingActors;
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}