using MediatR;
using MoviesApi.Data;

namespace MoviesApi.Features.Movies;

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