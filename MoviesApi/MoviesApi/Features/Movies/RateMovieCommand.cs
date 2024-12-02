using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Entities;

namespace MoviesApi.Features.Movies;

/// <summary>
///  Represents a command to assign a rating to a specific movie.
/// </summary>
/// <param name="Id">The unique identifier of the movie to be rated.</param>
/// <param name="Rate">The rating value. 0 ~ 5</param>
public record RateMovieCommandRequest(Guid Id, int Rate) : IRequest<bool>;

public class RateMovieCommandRequestValidator : AbstractValidator<RateMovieCommandRequest>
{
    public RateMovieCommandRequestValidator()
    {
        RuleFor(x => x.Rate).InclusiveBetween(0, 5);
    }
}

public class RateMovieCommandHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<RateMovieCommandRequest, bool>
{
    public async Task<bool> Handle(RateMovieCommandRequest request, CancellationToken cancellationToken)
    {
        var movie = await applicationDbContext.Movies
            .Include(x => x.Ratings)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (movie is null)
        {
            return false;
        }

        movie.Ratings.Add(new MovieRating(request.Rate));
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}