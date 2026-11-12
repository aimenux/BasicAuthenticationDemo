using Example05.Presentation.Authentication;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using static Example05.Tests.Helpers.AuthenticationHeaderBuilder;

namespace Example05.Tests.UnitTests;

public class BasicSecurityAlsoAgainAttributeTests
{
    [Fact]
    public async Task When_BasicHeader_Is_Valid_Then_Should_Call_Next()
    {
        // arrange
        var (executingContext, next) = BuildContexts(BuildBasicHeaderValue(BasicConstants.Username, BasicConstants.Password).ToString());
        var attribute = new BasicSecurityAlsoAgainAttribute();

        // act
        await attribute.OnActionExecutionAsync(executingContext, next.Invoke);

        // assert
        executingContext.Result.Should().BeNull();
        next.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task When_BasicHeader_Is_Missing_Then_Should_Return_Unauthorized_And_Not_Call_Next()
    {
        // arrange
        var (executingContext, next) = BuildContexts(null);
        var attribute = new BasicSecurityAlsoAgainAttribute();

        // act
        await attribute.OnActionExecutionAsync(executingContext, next.Invoke);

        // assert
        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
        next.WasCalled.Should().BeFalse();
    }

    [Theory]
    [InlineData("User", "Pass123")]
    [InlineData("UserAbc", "Pass")]
    public async Task When_BasicHeader_Is_Invalid_Then_Should_Return_Unauthorized_And_Not_Call_Next(string username, string password)
    {
        // arrange
        var (executingContext, next) = BuildContexts(BuildBasicHeaderValue(username, password).ToString());
        var attribute = new BasicSecurityAlsoAgainAttribute();

        // act
        await attribute.OnActionExecutionAsync(executingContext, next.Invoke);

        // assert
        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
        next.WasCalled.Should().BeFalse();
    }

    private static (ActionExecutingContext, NextProbe) BuildContexts(string authorization)
    {
        var httpContext = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
        if (authorization is not null)
        {
            httpContext.Request.Headers.Authorization = authorization;
        }

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), "controller");
        var executedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), "controller");
        return (executingContext, new NextProbe(executedContext));
    }

    private sealed class NextProbe(ActionExecutedContext executedContext)
    {
        public bool WasCalled { get; private set; }

        public Task<ActionExecutedContext> Invoke()
        {
            WasCalled = true;
            return Task.FromResult(executedContext);
        }
    }
}
