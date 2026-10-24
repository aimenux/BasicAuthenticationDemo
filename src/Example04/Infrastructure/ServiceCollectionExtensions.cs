using Example04.Infrastructure.Repositories;

namespace Example04.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMovieRepository, MovieRepository>();
        return builder;
    }
}