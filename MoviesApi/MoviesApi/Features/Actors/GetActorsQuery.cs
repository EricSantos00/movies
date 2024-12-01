using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Entities;
using MoviesApi.Features.Actors.Models;

namespace MoviesApi.Features.Actors;

public record GetActorsQueryRequest(string? Name) : IRequest<List<ActorViewModel>>;

public class GetActorsQueryHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<GetActorsQueryRequest, List<ActorViewModel>>
{
    public async Task<List<ActorViewModel>> Handle(GetActorsQueryRequest request, CancellationToken cancellationToken)
    {
        var actorsQuery = applicationDbContext.Actors.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Name))
            actorsQuery = actorsQuery.Where(actor => EF.Functions.Like(actor.Name, $"%{request.Name}%"));

        return await actorsQuery
            .Select(ToViewModel())
            .ToListAsync(cancellationToken);
    }

    private static Expression<Func<Actor, ActorViewModel>> ToViewModel()
    {
        return a => ActorViewModel.FromActor(a);
    }
}