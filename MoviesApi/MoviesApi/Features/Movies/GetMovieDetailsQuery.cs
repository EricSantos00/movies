using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Features.Movies.Models;

namespace MoviesApi.Features.Movies;

/// <summary>
/// Represents a query to fetch detailed information about a specific movie based on their unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the movie whose details are being requested.</param>
public record GetMovieDetailsQueryRequest(Guid Id) : IRequest<MovieDetailsViewModel?>;

public class GetMovieDetailsQueryHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<GetMovieDetailsQueryRequest, MovieDetailsViewModel?>
{
    public async Task<MovieDetailsViewModel?> Handle(GetMovieDetailsQueryRequest request,
        CancellationToken cancellationToken)
    {
        var movie = await applicationDbContext.Movies
            .AsNoTracking()
            .Include(x => x.Actors)
            .Include(x => x.Ratings)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return movie is null ? null : MovieDetailsViewModel.FromMovie(movie);
    }
}