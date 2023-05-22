using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Example07.Presentation.Authentication;

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<BasicAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options,
        logger,
        encoder,
        clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue(BasicConstants.BasicHeaderName, out var authorisationHeader))
        {
            return Task.FromResult(AuthenticateResult.Fail("Basic header is missing"));
        }
        
        var headerValue = authorisationHeader.ToString();
        if (!headerValue.StartsWith($"{BasicConstants.BasicScheme} ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Basic scheme is missing"));
        }
        
        if (!TryGetUserCredentials(headerValue, out var userCredentials))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization header is invalid"));
        }
        
        var (username, password) = userCredentials;
        if (username != BasicConstants.Username || password != BasicConstants.Password)
        {
            return Task.FromResult(AuthenticateResult.Fail("User credentials are invalid"));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username)
        };
        var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Options.AuthenticationScheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
    
    private static bool TryGetUserCredentials(string headerValue, out (string username, string password) userCredentials)
    {
        try
        {
            var encodedCredentials = headerValue.Split(' ', 2)[1];
            var credentialsBytes = Convert.FromBase64String(encodedCredentials);
            var credentialsString = Encoding.UTF8.GetString(credentialsBytes);
            var credentials = credentialsString.Split(':', 2);
            userCredentials = (credentials[0], credentials[1]);
            return true;
        }
        catch
        {
            userCredentials = (null, null);
            return false;
        }
    }
}