using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Entities;
using MoviesApi.Features.Actors.Models;

namespace MoviesApi.Features.Actors;

/// <summary>
/// Represents a query to fetch a list of actors, optionally filtered by a partial or full match of the actor's name.
/// </summary>
/// <param name="Name">An optional filter parameter for the actor's name. If null, all actors are returned; otherwise, the list is filtered based on the provided name.</param>
public record GetActorsQueryRequest(string? Name) : IRequest<List<ActorViewModel>>;

public class GetActorsQueryHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<GetActorsQueryRequest, List<ActorViewModel>>
{
    public Task<List<ActorViewModel>> Handle(GetActorsQueryRequest request, CancellationToken cancellationToken)
    {
        var actorsQuery = applicationDbContext.Actors.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Name))
            actorsQuery = actorsQuery.Where(actor => EF.Functions.Like(actor.Name, $"%{request.Name}%"));

        return actorsQuery
            .Select(ToViewModel())
            .ToListAsync(cancellationToken);
    }

    private static Expression<Func<Actor, ActorViewModel>> ToViewModel() =>
        a => ActorViewModel.FromActor(a);
}