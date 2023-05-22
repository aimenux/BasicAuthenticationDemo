using Microsoft.AspNetCore.Mvc;

namespace Example05.Presentation.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BasicSecurityAttribute : ServiceFilterAttribute
{
    public BasicSecurityAttribute() : base(typeof(BasicSecurityFilter))
    {
    }
}