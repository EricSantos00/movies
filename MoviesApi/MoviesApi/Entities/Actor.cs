namespace MoviesApi.Entities;

public class Actor : BaseEntity
{
    public Actor()
    {
    }

    public Actor(Guid id) : base(id)
    {
    }

    public required string Name { get; set; }
    public ISet<Movie> Movies { get; set; } = new HashSet<Movie>();
}