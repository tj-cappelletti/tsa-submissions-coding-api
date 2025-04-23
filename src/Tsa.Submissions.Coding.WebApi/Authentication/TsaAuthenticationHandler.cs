using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Authentication;

public class TsaAuthenticationHandler : AuthenticationHandler<TsaAuthenticationOptions>
{
    private readonly ICacheService _cacheService;
    private readonly string _systemApiToken;
    private readonly IUsersService _usersService;

    public TsaAuthenticationHandler(
        ICacheService cacheService,
        IConfiguration configuration,
        IUsersService usersService,
        IOptionsMonitor<TsaAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
        _cacheService = cacheService;
        _systemApiToken = configuration[ConfigurationKeys.SystemApiToken] ??
                          throw new InvalidOperationException($"The '{ConfigurationKeys.SystemApiToken}' configuration key is missing.");
        _usersService = usersService;
    }

    private AuthenticateResult AuthenticationFailure(string message)
    {
        Logger.LogWarning(message);
        return AuthenticateResult.Fail(message);
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticationFailure($"Missing header: {Options.ApiKeyHeaderName}");
        }

        if (apiKeyHeaderValues.Count > 1)
        {
            var message = $"Multiple headers: {Options.ApiKeyHeaderName}";
            Logger.LogWarning(message);
            return AuthenticationFailure($"Multiple headers: {Options.ApiKeyHeaderName}");
        }

        var apiKey = apiKeyHeaderValues[0];

        if (apiKey == null)
        {
            return AuthenticationFailure($"Missing header value: {Options.ApiKeyHeaderName}");
        }

        User user;
        if(apiKey == _systemApiToken)
        {

            user = new User
            {
                Id = "000000000000000000000000",
                Role = SubmissionRoles.Judge,
                UserName = "System"
            };
        }
        else
        {
            var userId = await _cacheService.GetAsync<string>(apiKey);

            if (userId == null)
            {
                return AuthenticationFailure($"Invalid {Options.ApiKeyHeaderName}");
            }

            user = await _usersService.GetAsync(userId) ??
                       throw new InvalidOperationException($"The user with ID '{userId}' was not found but an API key exists for them.");
        }

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
        Context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        var unauthorized = ApiErrorResponseModel.Unauthorized;

        await Context.Response.WriteAsJsonAsync(unauthorized);
    }
}
