using Example06.Infrastructure.Repositories;

namespace Example06.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMovieRepository, MovieRepository>();
        return builder;
    }
}