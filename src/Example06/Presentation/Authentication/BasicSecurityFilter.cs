using System.Security.Cryptography;
using System.Text;

namespace Example06.Presentation.Authentication;

public class BasicSecurityFilter : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(BasicConstants.BasicHeaderName, out var authorisationHeader))
        {
            return Unauthorized(context, "Basic header is missing");
        }

        var headerValue = authorisationHeader.ToString();
        if (!headerValue.StartsWith($"{BasicConstants.BasicScheme} ", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(context, "Basic scheme is missing");
        }

        if (!TryGetUserCredentials(headerValue, out var userCredentials))
        {
            return Unauthorized(context, "Authorization header is invalid");
        }

        var (username, password) = userCredentials;
        if (!IsValidUsername(username) | !IsValidPassword(password))
        {
            return Unauthorized(context, "User credentials are invalid");
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

    private static bool IsValidUsername(string username)
    {
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(username));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(BasicConstants.Username));
        return CryptographicOperations.FixedTimeEquals(suppliedHash, expectedHash);
    }

    private static bool IsValidPassword(string password)
    {
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(BasicConstants.Password));
        return CryptographicOperations.FixedTimeEquals(suppliedHash, expectedHash);
    }
    
    private static IResult Unauthorized(EndpointFilterInvocationContext context, string message)
    {
        context.HttpContext.Response.Headers.WWWAuthenticate = BasicConstants.BasicScheme;
        return HttpResults.Unauthorized(message);
    }
}