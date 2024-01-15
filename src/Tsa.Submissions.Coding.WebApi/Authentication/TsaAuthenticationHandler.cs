using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Authentication;

public class TsaAuthenticationHandler : AuthenticationHandler<TsaAuthenticationOptions>
{
    private readonly ICacheService _cacheService;
    private readonly IUsersService _usersService;

    public TsaAuthenticationHandler(
        ICacheService cacheService,
        IUsersService usersService,
        IOptionsMonitor<TsaAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
        _cacheService = cacheService;
        _usersService = usersService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.Fail($"Missing header: {Options.ApiKeyHeaderName}");
        }

        if (apiKeyHeaderValues.Count > 1)
        {
            return AuthenticateResult.Fail($"Multiple headers: {Options.ApiKeyHeaderName}");
        }

        var apiKey = apiKeyHeaderValues[0];

        if (apiKey == null)
        {
            return AuthenticateResult.Fail($"Missing header value: {Options.ApiKeyHeaderName}");
        }

        var userId = await _cacheService.GetAsync<string>(apiKey);

        if (userId == null)
        {
            return AuthenticateResult.Fail($"Invalid {Options.ApiKeyHeaderName}");
        }

        var user = await _usersService.GetAsync(userId) ?? throw new InvalidOperationException($"The user with ID '{userId}' was not found but an API key exists for them.");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Role, user.Role!)
        };

        var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var unauthorized = ApiErrorResponseModel.Unauthorized;

        await Context.Response.WriteAsJsonAsync(unauthorized);
    }
}
