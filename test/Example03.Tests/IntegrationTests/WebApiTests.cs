using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net;
using AwesomeAssertions;

namespace Example03.Tests.IntegrationTests;

public class WebApiTests
{
    [Theory]
    [InlineData("api/movies/1")]
    [InlineData("api/movies/list")]
    public async Task Should_Get_Movies_Returns_Success(string route)
    {
        // arrange
        await using var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();

        // act
        var response = await client.GetAsync(route);
        var responseBody = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseBody.Should().NotBeNullOrWhiteSpace();
    }
    
    [Theory]
    [InlineData("api/movies/1")]
    [InlineData("api/movies/list")]
    public async Task Should_Get_Movies_Returns_Unauthorized(string route)
    {
        // arrange
        var username = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        await using var fixture = new WebApiTestsFixture(username, password);
        var client = fixture.CreateClient();

        // act
        var response = await client.GetAsync(route);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_Get_Movies_Without_Authorization_Header_Returns_Unauthorized()
    {
        // arrange
        await using var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = null;

        // act
        var response = await client.GetAsync("api/movies/1");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_Get_Movies_With_Wrong_Password_Returns_Unauthorized()
    {
        // arrange
        await using var fixture = new WebApiTestsFixture(password: Guid.NewGuid().ToString());
        var client = fixture.CreateClient();

        // act
        var response = await client.GetAsync("api/movies/1");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_Get_Movies_With_Malformed_Base64_Returns_Unauthorized()
    {
        // arrange
        await using var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "not-base64!!");

        // act
        var response = await client.GetAsync("api/movies/1");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_Get_Movies_With_Malformed_Scheme_Returns_Unauthorized()
    {
        // arrange
        await using var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token");

        // act
        var response = await client.GetAsync("api/movies/1");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_Get_Unknown_Movie_Returns_NotFound()
    {
        // arrange
        await using var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();

        // act
        var response = await client.GetAsync("api/movies/999");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_Get_Movies_Over_Https_Returns_Hsts_Header()
    {
        // arrange
        await using var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://api.example.test")
        });

        // act
        var response = await client.GetAsync("api/movies/1");

        // assert
        response.Headers.Contains("Strict-Transport-Security").Should().BeTrue();
    }

    [Fact]
    public async Task Should_Get_Movies_Over_Http_Returns_No_Hsts_Header()
    {
        // arrange
        await using var fixture = new WebApiTestsFixture();
        var client = fixture.CreateClient();

        // act
        var response = await client.GetAsync("api/movies/1");

        // assert
        response.Headers.Contains("Strict-Transport-Security").Should().BeFalse();
    }
}