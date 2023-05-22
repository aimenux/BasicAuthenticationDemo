using System.Net;
using System.Text;

namespace Example02.Presentation.Authentication;

public class BasicMiddleware
{
    private readonly RequestDelegate _next;

    public BasicMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(BasicConstants.BasicHeaderName, out var authorisationHeader))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Authorization header is missing");
            return;
        }

        var headerValue = authorisationHeader.ToString();
        if (!headerValue.StartsWith($"{BasicConstants.BasicScheme} ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Authorization header is invalid");
            return;
        }

        if (!TryGetUserCredentials(headerValue, out var userCredentials))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Authorization header is invalid");
            return;
        }

        var (username, password) = userCredentials;
        if (username != BasicConstants.Username || password != BasicConstants.Password)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("User credentials are invalid");
            return;
        }
        
        await _next(context);
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

public static class AuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app)
    {
        app.UseMiddleware<BasicMiddleware>();
        return app;
    }
}