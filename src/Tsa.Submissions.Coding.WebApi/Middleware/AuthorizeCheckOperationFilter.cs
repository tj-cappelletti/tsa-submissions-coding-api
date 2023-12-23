using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tsa.Submissions.Coding.WebApi.Middleware;

// This is used for local dev work and does not need to be unit tested
[ExcludeFromCodeCoverage]
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    private static readonly string[] Item = ["tsa.coding.submissions.read", "tsa.coding.submissions.create"];

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    }
                ] = Item
            }
        };
    }
}
