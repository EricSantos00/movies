using MoviesApi.Extensions;
using MoviesApi.Features.Actors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(x => x.AddApiKeyAuthentication());
builder.Services.AddAppServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
}

app.UseHttpsRedirection();

app.MapGroup("/actors").MapActors();

app.Run();