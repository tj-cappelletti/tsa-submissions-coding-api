using Microsoft.AspNetCore.Authentication;

namespace Tsa.Submissions.Coding.WebApi.Authentication;

public class TsaAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "TsaApiKeyScheme";

    public string ApiKeyHeaderName { get; set; } = "X-API-Key";
}
