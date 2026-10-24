using Example04.Domain;

namespace Example04.Infrastructure.Repositories;

public interface IMovieRepository
{
    Task<IReadOnlyList<Movie>> GetMoviesAsync(CancellationToken cancellationToken);
    Task<Movie> GetMovieByIdAsync(int movieId, CancellationToken cancellationToken);
}