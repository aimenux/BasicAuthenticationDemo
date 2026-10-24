using Example06.Infrastructure;
using Example06.Presentation;
using Example06.Presentation.Authentication;
using Example06.Presentation.Endpoints;

var app = WebApplication.CreateBuilder(args)
    .AddPresentation()
    .AddInfrastructure()
    .Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app
    .MapGet("/api/movies/list", (IMoviesEndpoints endpoints, CancellationToken cancellationToken) => endpoints.GetMoviesAsync(cancellationToken))
    .AddEndpointFilter<BasicSecurityFilter>()
    .WithName("GetMovies");

app
    .MapGet("/api/movies/{movieId:int}", (IMoviesEndpoints endpoints, int movieId, CancellationToken cancellationToken) => endpoints.GetMovieByIdAsync(movieId, cancellationToken))
    .AddEndpointFilter<BasicSecurityFilter>()
    .WithName("GetMovieById");

app.Run();