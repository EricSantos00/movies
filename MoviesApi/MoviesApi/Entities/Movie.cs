namespace MoviesApi.Entities;

public class Movie : BaseEntity
{
    public Movie()
    {
        
    }

    public Movie(Guid id) : base(id)
    {
        
    }
    
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime ReleaseDate { get; set; }
    public ISet<Actor> Actors { get; set; } = new HashSet<Actor>();
    public List<MovieRating> Ratings { get; set; } = [];
}