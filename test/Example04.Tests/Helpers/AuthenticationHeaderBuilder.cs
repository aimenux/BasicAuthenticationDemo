using System.Net.Http.Headers;
using System.Text;

namespace Example04.Tests.Helpers;

public static class AuthenticationHeaderBuilder
{
    public static AuthenticationHeaderValue BuildBasicHeaderValue(string username, string password)
    {
        var authenticationString = $"{username}:{password}";
        var authenticationBytes = Encoding.UTF8.GetBytes(authenticationString);
        var base64String = Convert.ToBase64String(authenticationBytes);
        return new AuthenticationHeaderValue("Basic", base64String);
    }
}