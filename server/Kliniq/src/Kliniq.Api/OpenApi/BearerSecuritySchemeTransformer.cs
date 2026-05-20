using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Kliniq.Api.OpenApi
{
    public sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

            if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
            {
                document.Components ??= new OpenApiComponents();

                var bearerScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                };

                document.AddComponent("Bearer", bearerScheme);

                var cookieScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "accessToke",
                    Description = "HTTPOnly JWT cookie — set automatically after login/register."
                };

                document.AddComponent("CookieAuth", cookieScheme);

                var bearerRequirement = new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                };
                
                var cookieRequirement = new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("CookieAuth", document)] = []
                };

                foreach (var operation in document.Paths.Values.SelectMany(p => p.Operations!))
                {
                    operation.Value.Security ??= new List<OpenApiSecurityRequirement>();
                    operation.Value.Security.Add(bearerRequirement); 
                    operation.Value.Security.Add(cookieRequirement);
                }
            }
        }
    }
}
