using MoviesApi.Entities;
using MoviesApi.Features.Movies.Models;

namespace MoviesApi.Features.Actors.Models;

public record ActorsViewModel(Guid Id, string Name)
{
    public static ActorsViewModel FromActor(Actor actor) => new(actor.Id, actor.Name);
}

public record ActorsDetailsViewModel(Guid Id, string Name, IReadOnlyCollection<MovieViewModel> Movies)
{
    public static ActorsDetailsViewModel FromActor(Actor actor) =>
        new(actor.Id, actor.Name, actor.Movies.Select(MovieViewModel.FromMovie).ToList());
}