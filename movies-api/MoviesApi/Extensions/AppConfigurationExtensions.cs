using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Features.Actors;
using MoviesApi.Validation;

namespace MoviesApi.Extensions;

public static class AppConfigurationExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(x => x.UseSqlite("Data source=movies.db"));

        services.AddMediatR(x =>
        {
            x.AddOpenBehavior(typeof(ValidationBehavior<,>));
            x.RegisterServicesFromAssemblyContaining<CreateActorCommandRequest>();
        });
        services.AddValidatorsFromAssemblyContaining(typeof(CreateActorCommandRequestValidator));

        return services;
    }
}