using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using BC = BCrypt.Net.BCrypt;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AuthenticationController : ControllerBase
{
    private const int ApiKeyLength = 64;

    private readonly ICacheService _cacheService;
    private readonly IUsersService _usersService;

    public AuthenticationController(ICacheService cacheService, IUsersService usersService)
    {
        _cacheService = cacheService;
        _usersService = usersService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Post(AuthenticationModel authenticationModel, CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetByUserNameAsync(authenticationModel.UserName);

        if (user == null) return Unauthorized();

        if (!BC.Verify(authenticationModel.Password, user.PasswordHash)) return Unauthorized();

        // If the ID of the user is null, then we are in a bad state.
        var userId = user.Id!;

        var apiKey = await _cacheService.GetAsync<string?>(userId, cancellationToken);

        if (apiKey != null)
        {
            await _cacheService.RemoveAsync(apiKey, cancellationToken);
        }

        apiKey = GenerateApiKey();

        await _cacheService.SetAsync(apiKey, userId, new TimeSpan(0,2,0,0), cancellationToken);
        await _cacheService.SetAsync(userId, apiKey, new TimeSpan(0,2,0,0), cancellationToken);

        string? uri = null;
        return Created(uri, new AuthenticationModel
        {
            ApiKey = apiKey,
            UserName = user.UserName
        });
    }

    private static string GenerateApiKey()
    {
        ReadOnlySpan<char> apiKeyCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        return RandomNumberGenerator.GetString(apiKeyCharacters, ApiKeyLength);
    }
}
