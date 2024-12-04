using MoviesApi.Data.Seeders;
using MoviesApi.Extensions;
using MoviesApi.Features.Actors;
using MoviesApi.Features.Movies;
using MoviesApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddAppServices();
builder.Services.AddCors(x =>
{
    x.AddPolicy("CorsPolicy", options =>
        options.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// This should be used only in development
await app.Services.SeedDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapSwagger();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();

app.Run();