using Example06.Presentation.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using static Example06.Tests.Helpers.AuthenticationHeaderBuilder;

namespace Example06.Tests.UnitTests;

public class BasicSecurityFilterTests
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

        var filterContext = new FilterContext(context);
        var securityFilter = new BasicSecurityFilter();

        // act
        var result = await securityFilter.InvokeAsync(filterContext, _ => new ValueTask<object>(Results.Ok()));

        // assert
        result.Should().BeOfType<Ok>();
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

        var filterContext = new FilterContext(context);
        var securityFilter = new BasicSecurityFilter();

        // act
        var result = await securityFilter.InvokeAsync(filterContext, _ => new ValueTask<object>());

        // assert
        result.Should().BeOfType<HttpResults.UnauthorizedHttpResultWithResponseBody>();
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

        var filterContext = new FilterContext(context);
        var securityFilter = new BasicSecurityFilter();

        // act
        var result = await securityFilter.InvokeAsync(filterContext, _ => new ValueTask<object>());

        // assert
        result.Should().BeOfType<HttpResults.UnauthorizedHttpResultWithResponseBody>();
    }

    private class FilterContext : EndpointFilterInvocationContext
    {
        public FilterContext(HttpContext httpContext)
        {
            HttpContext = httpContext;
            Arguments = new List<object>();
        }

        public override T GetArgument<T>(int index)
        {
            return default;
        }

        public override HttpContext HttpContext { get; }
        public override IList<object> Arguments { get; }
    }
}