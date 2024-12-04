using MoviesApi.Entities;
using MoviesApi.Features.Actors.Models;

namespace MoviesApi.Features.Movies.Models;

public record MovieViewModel(Guid Id, string Title, string Description, DateTime ReleaseDate, double AverageRating)
{
    public static MovieViewModel FromMovie(Movie movie) =>
        new(movie.Id, movie.Title, movie.Description, movie.ReleaseDate,
            movie.Ratings.Count > 0 ? movie.Ratings.Average(x => x.Rating) : 0);
}

public record MovieDetailsViewModel(
    Guid Id,
    string Title,
    string Description,
    DateTime ReleaseDate,
    double AverageRating,
    IReadOnlyCollection<ActorViewModel> Actors)
{
    public static MovieDetailsViewModel FromMovie(Movie movie) =>
        new(movie.Id, movie.Title, movie.Description, movie.ReleaseDate,
            movie.Ratings.Count > 0 ? movie.Ratings.Average(x => x.Rating) : 0,
            movie.Actors.Select(ActorViewModel.FromActor).ToList());
}