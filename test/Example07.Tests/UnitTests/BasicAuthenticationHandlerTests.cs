using System.Security.Claims;
using System.Text.Encodings.Web;
using AwesomeAssertions;
using Example07.Presentation.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using static Example07.Tests.Helpers.AuthenticationHeaderBuilder;

namespace Example07.Tests.UnitTests;

public class BasicAuthenticationHandlerTests
{
    [Fact]
    public async Task When_BasicHeader_Is_Valid_Then_Should_Succeed_With_Username_Claim()
    {
        // arrange
        var header = BuildBasicHeaderValue(BasicConstants.Username, BasicConstants.Password).ToString();
        var (handler, _) = await BuildHandlerAsync(header);

        // act
        var result = await handler.AuthenticateAsync();

        // assert
        result.Succeeded.Should().BeTrue();
        result.Principal.Identity.IsAuthenticated.Should().BeTrue();
        result.Principal.FindFirst(ClaimTypes.Name).Value.Should().Be(BasicConstants.Username);
    }

    [Fact]
    public async Task When_BasicHeader_Is_Missing_Then_Should_Fail()
    {
        // arrange
        var (handler, _) = await BuildHandlerAsync(null);

        // act
        var result = await handler.AuthenticateAsync();

        // assert
        result.Succeeded.Should().BeFalse();
        result.Failure.Message.Should().Be("Basic header is missing");
    }

    [Fact]
    public async Task When_Scheme_Is_Not_Basic_Then_Should_Fail()
    {
        // arrange
        var (handler, _) = await BuildHandlerAsync("Bearer abc");

        // act
        var result = await handler.AuthenticateAsync();

        // assert
        result.Succeeded.Should().BeFalse();
        result.Failure.Message.Should().Be("Basic scheme is missing");
    }

    [Theory]
    [InlineData("Basic !!!not-base64!!!")]
    [InlineData("Basic dXNlcldpdGhvdXRDb2xvbg==")] // "userWithoutColon"
    public async Task When_BasicHeader_Is_Malformed_Then_Should_Fail(string header)
    {
        // arrange
        var (handler, _) = await BuildHandlerAsync(header);

        // act
        var result = await handler.AuthenticateAsync();

        // assert
        result.Succeeded.Should().BeFalse();
        result.Failure.Message.Should().Be("Authorization header is invalid");
    }

    [Theory]
    [InlineData("User", "Pass123")]
    [InlineData("UserAbc", "Pass")]
    public async Task When_Credentials_Are_Invalid_Then_Should_Fail(string username, string password)
    {
        // arrange
        var (handler, _) = await BuildHandlerAsync(BuildBasicHeaderValue(username, password).ToString());

        // act
        var result = await handler.AuthenticateAsync();

        // assert
        result.Succeeded.Should().BeFalse();
        result.Failure.Message.Should().Be("User credentials are invalid");
    }

    [Fact]
    public async Task When_Challenged_Then_Should_Return_401_With_WwwAuthenticate_Header()
    {
        // arrange
        var (handler, context) = await BuildHandlerAsync(null);

        // act
        await handler.ChallengeAsync(new AuthenticationProperties());

        // assert
        context.Response.StatusCode.Should().Be(401);
        context.Response.Headers.WWWAuthenticate.ToString().Should().Be(BasicConstants.BasicScheme);
    }

    private static async Task<(BasicAuthenticationHandler, HttpContext)> BuildHandlerAsync(string authorization)
    {
        var options = Substitute.For<IOptionsMonitor<BasicAuthenticationOptions>>();
        options.Get(BasicConstants.BasicScheme).Returns(new BasicAuthenticationOptions());

        var context = new DefaultHttpContext();
        if (authorization is not null)
        {
            context.Request.Headers.Authorization = authorization;
        }

        var handler = new BasicAuthenticationHandler(options, NullLoggerFactory.Instance, UrlEncoder.Default);
        var scheme = new AuthenticationScheme(BasicConstants.BasicScheme, null, typeof(BasicAuthenticationHandler));
        await handler.InitializeAsync(scheme, context);
        return (handler, context);
    }
}
