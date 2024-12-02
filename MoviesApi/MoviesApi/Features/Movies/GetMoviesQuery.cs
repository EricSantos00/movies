﻿using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Entities;
using MoviesApi.Features.Movies.Models;

namespace MoviesApi.Features.Movies;

public record GetMoviesQueryRequest(string? Title) : IRequest<List<MovieViewModel>>;

public class GetMoviesQueryHandler(ApplicationDbContext applicationDbContext)
    : IRequestHandler<GetMoviesQueryRequest, List<MovieViewModel>>
{
    public Task<List<MovieViewModel>> Handle(GetMoviesQueryRequest request, CancellationToken cancellationToken)
    {
        var moviesQuery = applicationDbContext.Movies.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Title))
            moviesQuery = moviesQuery.Where(movie => EF.Functions.Like(movie.Title, $"%{request.Title}%"));

        return moviesQuery
            .Include(x => x.Ratings)
            .Select(ToViewModel())
            .ToListAsync(cancellationToken);
    }

    private static Expression<Func<Movie, MovieViewModel>> ToViewModel() =>
        m => MovieViewModel.FromMovie(m);
}