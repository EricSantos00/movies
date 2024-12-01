namespace MoviesApi.Entities;

public class Actor : BaseEntity
{
    public required string Name { get; set; }
    public List<Movie> Movies { get; set; } = [];
}