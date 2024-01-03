using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tsa.Submissions.Coding.WebApi.Authentication;

namespace Tsa.Submissions.Coding.WebApi.Swagger;

// This should be excluded from code coverage
// It is only used during development and local testing
[ExcludeFromCodeCoverage]
public class ApiKeyHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.CustomAttributes.All(attribute => attribute.AttributeType != typeof(AuthorizeAttribute))) return;

        var tsaAuthenticationOptions = new TsaAuthenticationOptions();

        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Description = "The API key to use for authentication. This is generated from the Authentication endpoint.",
            In = ParameterLocation.Header,
            Name = tsaAuthenticationOptions.ApiKeyHeaderName,
            Required = true
        });
    }
}
