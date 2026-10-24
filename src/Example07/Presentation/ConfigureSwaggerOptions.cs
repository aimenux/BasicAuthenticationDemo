using Example07.Presentation.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Example07.Presentation;

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

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(BasicConstants.BasicId, document)] = []
        });
    }
}
