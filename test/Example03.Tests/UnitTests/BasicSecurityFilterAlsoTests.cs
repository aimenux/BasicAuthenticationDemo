using Example03.Presentation.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using static Example03.Tests.Helpers.AuthenticationHeaderBuilder;

namespace Example03.Tests.UnitTests;

public class BasicSecurityFilterAlsoTests
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

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), "controller");
        var executedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), "controller");
        var securityFilter = new BasicSecurityFilterAlso();

        // act
        await securityFilter.OnActionExecutionAsync(executingContext, () => Task.FromResult(executedContext));

        // assert
        executingContext.Result.Should().BeNull();
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

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), "controller");
        var executedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), "controller");
        var securityFilter = new BasicSecurityFilterAlso();

        // act
        await securityFilter.OnActionExecutionAsync(executingContext, () => Task.FromResult(executedContext));

        // assert
        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
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

        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), "controller");
        var executedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), "controller");
        var securityFilter = new BasicSecurityFilterAlso();

        // act
        await securityFilter.OnActionExecutionAsync(executingContext, () => Task.FromResult(executedContext));

        // assert
        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}