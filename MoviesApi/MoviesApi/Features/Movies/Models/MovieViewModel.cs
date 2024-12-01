using MoviesApi.Entities;

namespace MoviesApi.Features.Movies.Models;

public record MovieViewModel(Guid Id, string Title, string Description, DateTime ReleaseDate, double AverageRating)
{
    public static MovieViewModel FromMovie(Movie movie) =>
        new(movie.Id, movie.Title, movie.Description, movie.ReleaseDate,
            movie.Ratings.Average(x => x.Rating));
}