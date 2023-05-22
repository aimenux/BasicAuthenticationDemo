using Microsoft.AspNetCore.Authentication;

namespace Example08.Presentation.Authentication;

public static class SecurityExtensions
{
    public static IServiceCollection AddBasicAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = BasicConstants.BasicScheme;
                options.DefaultChallengeScheme = BasicConstants.BasicScheme;
                options.DefaultAuthenticateScheme = BasicConstants.BasicScheme;
            })
            .AddBasicScheme(options =>
            {
                options.AuthenticationScheme = BasicConstants.BasicScheme;
                options.AuthenticationType = BasicConstants.BasicScheme;
            });

        return services;
    }
    
    private static AuthenticationBuilder AddBasicScheme(this AuthenticationBuilder authenticationBuilder, Action<BasicAuthenticationOptions> options)
    {
        return authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(BasicConstants.BasicScheme, options);
    }
}