﻿using Microsoft.AspNetCore.Authentication;

namespace Example07.Presentation.Authentication;

public class BasicAuthenticationOptions : AuthenticationSchemeOptions
{
    public string AuthenticationScheme { get; set; } = BasicConstants.BasicScheme;
    public string AuthenticationType { get; set; } = BasicConstants.BasicScheme;
}