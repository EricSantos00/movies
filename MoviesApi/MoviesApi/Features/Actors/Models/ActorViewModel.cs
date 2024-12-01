using MoviesApi.Entities;
using MoviesApi.Features.Movies.Models;

namespace MoviesApi.Features.Actors.Models;

public record ActorViewModel(Guid Id, string Name)
{
    public static ActorViewModel FromActor(Actor actor) => new(actor.Id, actor.Name);
}

public record ActorDetailsViewModel(Guid Id, string Name, IReadOnlyCollection<MovieViewModel> Movies)
{
    public static ActorDetailsViewModel FromActor(Actor actor) =>
        new(actor.Id, actor.Name, actor.Movies.Select(MovieViewModel.FromMovie).ToList());
}