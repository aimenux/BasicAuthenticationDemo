using Microsoft.AspNetCore.Mvc;

namespace Example05.Presentation.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BasicSecurityAlsoAttribute : TypeFilterAttribute
{
    public BasicSecurityAlsoAttribute() : base(typeof(BasicSecurityFilter))
    {
    }
}