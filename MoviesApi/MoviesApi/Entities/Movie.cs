namespace MoviesApi.Entities;

public class Movie : BaseEntity
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Genre { get; set; }
    public required string Director { get; set; }
    public required DateTime ReleaseDate { get; set; }
    public List<Actor> Actors { get; set; } = [];
    public List<MovieRating> Ratings { get; set; } = [];
}