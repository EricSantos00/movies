using MoviesApi.Entities;

namespace MoviesApi.Features.Actors.Models;

public record ActorDetailsViewModel(Guid Id, string Name, IReadOnlyCollection<MovieViewModel> Movies)
{
    public static ActorDetailsViewModel FromActor(Actor actor) =>
        new(actor.Id, actor.Name, actor.Movies.Select(MovieViewModel.FromMovie).ToList());
}