namespace MoviesApi.Entities;

public class Actor : BaseEntity
{
    public required string Name { get; set; }
    public ISet<Movie> Movies { get; set; } = new HashSet<Movie>();
}