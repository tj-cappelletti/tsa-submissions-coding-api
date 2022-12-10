using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tsa.Submissions.Coding.WebApi.Middleware;

// This is used for local dev work and does not need to be unit tested
[ExcludeFromCodeCoverage]
public class AuthorizeCheckOperationFilter : IOperationFilter
{
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
                ] = new[] { "tsa.coding.submissions.read", "tsa.coding.submissions.create" }
            }
        };
    }
}
