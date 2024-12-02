using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;

namespace MoviesApi.Features.Movies;

/// <summary>
/// Represents a command with the data required to update a movie details
/// </summary>
/// <param name="Id">The unique identifier of the movie to be updated.</param>
/// <param name="Title">The updated title of the movie.</param>
/// <param name="Description">The updated description of the movie.</param>
/// <param name="ReleaseDate">The updated release date of the movie.</param>
/// <param name="Actors">An optional list of unique identifiers for the actors associated with the movie.</param>
public record UpdateMovieCommandRequest(
    [property: JsonIgnore] Guid Id,
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