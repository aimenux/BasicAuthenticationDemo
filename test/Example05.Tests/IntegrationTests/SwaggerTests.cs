using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using AwesomeAssertions;

namespace Example05.Tests.IntegrationTests;

public class SwaggerTests
{
    private const string SwaggerJsonRoute = "swagger/v1/swagger.json";

    [Fact]
    public async Task Should_Get_Swagger_Document_Returns_Success()
    {
        // arrange
        await using var fixture = CreateDevelopmentFixture();
        var client = fixture.CreateClient();

        // act
        var response = await client.GetAsync(SwaggerJsonRoute);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task Should_Swagger_Document_Contains_Movies_Routes()
    {
        // arrange
        await using var fixture = CreateDevelopmentFixture();
        var client = fixture.CreateClient();

        // act
        var json = await client.GetStringAsync(SwaggerJsonRoute);
        using var document = JsonDocument.Parse(json);

        // assert
        document.RootElement.TryGetProperty("openapi", out _).Should().BeTrue();
        document.RootElement.TryGetProperty("paths", out var paths).Should().BeTrue();
        var routes = paths.EnumerateObject().Select(x => x.Name).ToList();
        routes.Should().BeEquivalentTo("/api/movies/list", "/api/movies/{movieId}");
    }

    [Fact]
    public async Task Should_Swagger_Document_Declares_Basic_Security_Scheme()
    {
        // arrange
        await using var fixture = CreateDevelopmentFixture();
        var client = fixture.CreateClient();

        // act
        var json = await client.GetStringAsync(SwaggerJsonRoute);
        using var document = JsonDocument.Parse(json);

        // assert
        var scheme = document.RootElement
            .GetProperty("components")
            .GetProperty("securitySchemes")
            .GetProperty("Basic");
        scheme.GetProperty("type").GetString().Should().Be("http");
        scheme.GetProperty("scheme").GetString().Should().Be("Basic");
    }

    [Fact]
    public async Task Should_Get_Swagger_Ui_Returns_Success()
    {
        // arrange
        await using var fixture = CreateDevelopmentFixture();
        var client = fixture.CreateClient();

        // act
        var response = await client.GetAsync("swagger/index.html");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Swagger is only mapped in the Development environment
    private static WebApplicationFactory<Program> CreateDevelopmentFixture()
    {
        return new WebApiTestsFixture().WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
    }
}
