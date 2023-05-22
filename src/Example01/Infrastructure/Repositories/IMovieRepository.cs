﻿using Example01.Domain;

namespace Example01.Infrastructure.Repositories;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetMoviesAsync(CancellationToken cancellationToken);
    Task<Movie> GetMovieByIdAsync(int movieId, CancellationToken cancellationToken);
}