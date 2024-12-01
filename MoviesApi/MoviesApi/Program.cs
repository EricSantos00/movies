using MoviesApi.Data.Seeders;
using MoviesApi.Extensions;
using MoviesApi.Features.Actors;
using MoviesApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(x => x.AddApiKeyAuthentication());
builder.Services.AddAppServices();

var app = builder.Build();
await app.Services.SeedDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapGroup("/actors").MapActors();

app.Run();