using Example01.Domain;
using Example01.Infrastructure.Repositories;
using Example01.Presentation.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Example01.Tests.UnitTests;

public class MoviesControllerTests
{
    [Fact]
    public async Task Should_Get_Movies_Returns_Success()
    {
        // arrange
        var repository = Substitute.For<IMovieRepository>();
        repository
            .GetMoviesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Movie>
            {
                new Movie
                {
                    Id = 1,
                    Title = "Matrix"
                }
            });
        var logger = NullLogger<MoviesController>.Instance;
        var controller = new MoviesController(repository, logger);

        // act
        var result = await controller.GetMoviesAsync(CancellationToken.None);

        // assert
        result.Should().BeOfType<OkObjectResult>();
        result
            .As<OkObjectResult>().Value
            .As<IEnumerable<Movie>>()
            .Should().NotBeNullOrEmpty();
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task Should_Get_Movie_By_Id_Returns_Success(int movieId)
    {
        // arrange
        var repository = Substitute.For<IMovieRepository>();
        repository
            .GetMovieByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new Movie
            {
                Id = 1,
                Title = "Matrix"
            });
        var logger = NullLogger<MoviesController>.Instance;
        var controller = new MoviesController(repository, logger);

        // act
        var result = await controller.GetMovieByIdAsync(movieId, CancellationToken.None);

        // assert
        result.Should().BeOfType<OkObjectResult>();
        result
            .As<OkObjectResult>().Value
            .As<Movie>()
            .Should().NotBeNull();
    }    
}