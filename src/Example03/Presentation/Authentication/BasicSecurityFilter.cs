using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Example03.Presentation.Authentication;

public class BasicSecurityFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(BasicConstants.BasicHeaderName, out var authorisationHeader))
        {
            context.Result = new UnauthorizedObjectResult("Basic header is missing");
            return;
        }

        var headerValue = authorisationHeader.ToString();
        if (!headerValue.StartsWith($"{BasicConstants.BasicScheme} ", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedObjectResult("Basic scheme is missing");
            return;
        }

        if (!TryGetUserCredentials(headerValue, out var userCredentials))
        {
            context.Result = new UnauthorizedObjectResult("Authorization header is invalid");
            return;
        }

        var (username, password) = userCredentials;
        if (username != BasicConstants.Username || password != BasicConstants.Password)
        {
            context.Result = new UnauthorizedObjectResult("User credentials are invalid");
        }
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