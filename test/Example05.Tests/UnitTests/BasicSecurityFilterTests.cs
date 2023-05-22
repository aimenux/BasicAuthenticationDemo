using Example05.Presentation.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using static Example05.Tests.Helpers.AuthenticationHeaderBuilder;

namespace Example05.Tests.UnitTests;

public class BasicSecurityFilterTests
{
    [Fact]
    public void When_BasicHeader_Is_Valid_Then_Should_Returns_Ok()
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
        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        var securityFilter = new BasicSecurityFilter();

        // act
        securityFilter.OnAuthorization(filterContext);

        // assert
        filterContext.Result.Should().BeNull();
    }
    
    [Fact]
    public void When_BasicHeader_Is_Missing_Then_Should_Returns_Unauthorized()
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
        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        var securityFilter = new BasicSecurityFilter();

        // act
        securityFilter.OnAuthorization(filterContext);

        // assert
        filterContext.Result.Should().NotBeNull();
        filterContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
    
    [Theory]
    [InlineData("User", "Pass123")]
    [InlineData("UserAbc", "Pass")]
    public void When_BasicHeader_Is_Invalid_Then_Should_Returns_Unauthorized(string username, string password)
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
        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        var securityFilter = new BasicSecurityFilter();

        // act
        securityFilter.OnAuthorization(filterContext);

        // assert
        filterContext.Result.Should().NotBeNull();
        filterContext.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}