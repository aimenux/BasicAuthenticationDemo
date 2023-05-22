using Example04.Presentation.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Example04.Presentation;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition(BasicConstants.BasicId, new OpenApiSecurityScheme
        {
            Description = "The credentials to access the API",
            Name = BasicConstants.BasicHeaderName,
            Scheme = BasicConstants.BasicScheme,
            Type = SecuritySchemeType.Http,
            In = ParameterLocation.Header
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = BasicConstants.BasicId,
                        Type = ReferenceType.SecurityScheme
                    },
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    }
}