using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using static Example07.Tests.Helpers.AuthenticationHeaderBuilder;
using static Example07.Presentation.Authentication.BasicConstants;

namespace Example07.Tests.IntegrationTests;

internal class WebApiTestsFixture : WebApplicationFactory<Program>
{
    private readonly string _username;
    private readonly string _password;
    
    public WebApiTestsFixture(string username = Username, string password = Password)
    {
        _username = username;
        _password = password;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
        });

        builder.ConfigureTestServices(services =>
        {
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        var basicHeaderValue = BuildBasicHeaderValue(_username, _password);
        client
            .DefaultRequestHeaders
            .Authorization = basicHeaderValue;
    }
}