using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Features.Actors;
using MoviesApi.Validations;

namespace MoviesApi.Extensions;

public static class AppConfigurationExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase("MoviesDb"));

        services.AddMediatR(x =>
        {
            x.AddOpenBehavior(typeof(ValidationBehavior<,>));
            x.RegisterServicesFromAssemblyContaining<CreateActorCommandRequest>();
        });
        services.AddValidatorsFromAssemblyContaining(typeof(CreateActorCommandRequestValidator));

        return services;
    }
}