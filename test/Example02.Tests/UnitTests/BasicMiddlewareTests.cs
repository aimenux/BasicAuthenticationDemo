using Example02.Presentation.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using static Example02.Tests.Helpers.AuthenticationHeaderBuilder;

namespace Example02.Tests.UnitTests;

public class BasicMiddlewareTests
{
    [Fact]
    public async Task When_BasicHeader_Is_Valid_Then_Should_Returns_Ok()
    {
        // arrange
        const string username = BasicConstants.Username;
        const string password = BasicConstants.Password;
        
        var context = new DefaultHttpContext
        {
            Request = { Headers = { Authorization = $"{BuildBasicHeaderValue(username, password)}" } },
            Response =
            {
                Body = new MemoryStream()
            }
        };

        Task Next(HttpContext _) => Task.CompletedTask;
        var middleware = new BasicMiddleware(Next);

        // act
        await middleware.InvokeAsync(context);

        // assert
        context.Response.StatusCode.Should().Be(200);
    }
    
    [Fact]
    public async Task When_BasicHeader_Is_Missing_Then_Should_Returns_Unauthorized()
    {
        // arrange
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        Task Next(HttpContext _) => Task.CompletedTask;
        var middleware = new BasicMiddleware(Next);

        // act
        await middleware.InvokeAsync(context);

        // assert
        context.Response.StatusCode.Should().Be(401);
    }
    
    [Theory]
    [InlineData("User", "Pass123")]
    [InlineData("UserAbc", "Pass")]
    public async Task When_BasicHeader_Is_Invalid_Then_Should_Returns_Unauthorized(string username, string password)
    {
        // arrange
        var context = new DefaultHttpContext
        {
            Request = { Headers = { Authorization = $"{BuildBasicHeaderValue(username, password)}" } },
            Response =
            {
                Body = new MemoryStream()
            }
        };

        Task Next(HttpContext _) => Task.CompletedTask;
        var middleware = new BasicMiddleware(Next);

        // act
        await middleware.InvokeAsync(context);

        // assert
        context.Response.StatusCode.Should().Be(401);
    }
}