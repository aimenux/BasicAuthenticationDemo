using Example02.Infrastructure.Repositories;

namespace Example02.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMovieRepository, MovieRepository>();
        return builder;
    }
}