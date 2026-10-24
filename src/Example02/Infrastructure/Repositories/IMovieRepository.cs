using Example02.Domain;

namespace Example02.Infrastructure.Repositories;

public interface IMovieRepository
{
    Task<IReadOnlyList<Movie>> GetMoviesAsync(CancellationToken cancellationToken);
    Task<Movie> GetMovieByIdAsync(int movieId, CancellationToken cancellationToken);
}