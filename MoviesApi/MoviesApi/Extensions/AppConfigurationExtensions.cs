using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Features.Actors;

namespace MoviesApi.Extensions;

public static class AppConfigurationExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase("MoviesDb"));

        services.AddMediatR(x => { x.RegisterServicesFromAssemblyContaining<CreateActorCommandRequest>(); });
        services.AddValidatorsFromAssemblyContaining(typeof(CreateActorCommandRequestValidator));

        return services;
    }
}