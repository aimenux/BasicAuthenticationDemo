using System.Text;

namespace Example06.Presentation.Authentication;

public class BasicSecurityFilter : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(BasicConstants.BasicHeaderName, out var authorisationHeader))
        {
            return HttpResults.Unauthorized("Basic header is missing");
        }

        var headerValue = authorisationHeader.ToString();
        if (!headerValue.StartsWith($"{BasicConstants.BasicScheme} ", StringComparison.OrdinalIgnoreCase))
        {
            return HttpResults.Unauthorized("Basic scheme is missing");
        }

        if (!TryGetUserCredentials(headerValue, out var userCredentials))
        {
            return HttpResults.Unauthorized("Authorization header is invalid");
        }

        var (username, password) = userCredentials;
        if (username != BasicConstants.Username || password != BasicConstants.Password)
        {
            return HttpResults.Unauthorized("User credentials are invalid");
        }
        
        return await next(context);
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