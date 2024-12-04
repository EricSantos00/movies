using MediatR;
using MoviesApi.Data;

namespace MoviesApi.Features.Movies;

/// <summary>
/// Represents a command to delete a movie from the system.
/// </summary>
/// <param name="Id">The unique identifier of the movie to be deleted.</param>
public record DeleteMovieCommandRequest(Guid Id) : IRequest<bool>;

public class DeleteMovieCommandHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<DeleteMovieCommandRequest, bool>
{
    public async Task<bool> Handle(DeleteMovieCommandRequest request, CancellationToken cancellationToken)
    {
        var movie = await applicationDbContext.Movies.FindAsync([request.Id], cancellationToken);
        if (movie is null)
        {
            return false;
        }

        applicationDbContext.Movies.Remove(movie);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}