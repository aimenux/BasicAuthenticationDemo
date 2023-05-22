﻿using Example02.Domain;

namespace Example02.Infrastructure.Repositories;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetMoviesAsync(CancellationToken cancellationToken);
    Task<Movie> GetMovieByIdAsync(int movieId, CancellationToken cancellationToken);
}