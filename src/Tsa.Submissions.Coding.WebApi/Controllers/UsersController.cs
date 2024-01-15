using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using BC = BCrypt.Net.BCrypt;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly ICacheService _cacheService;
    private readonly IUsersService _usersService;

    public UsersController(ICacheService cacheService, IUsersService usersService)
    {
        _cacheService = cacheService;
        _usersService = usersService;
    }

    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetAsync(id, cancellationToken);

        if (user == null) return NotFound();

        await _usersService.RemoveAsync(user, cancellationToken);

        return NoContent();
    }

    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserModel>))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var users = await _usersService.GetAsync(cancellationToken);

        return Ok(users.ToModels());
    }

    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetAsync(id, cancellationToken);

        if (user == null) return NotFound();

        return Ok(user.ToModel());
    }

    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Post(UserModel userModel, CancellationToken cancellationToken = default)
    {
        var passwordHash = BC.HashPassword(userModel.Password);

        var user = userModel.ToEntity(passwordHash);

        await _usersService.CreateAsync(user, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel());
    }

    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Post(UserModel[] userModels, CancellationToken cancellationToken = default)
    {
        foreach (var userModel in userModels)
        {
            await Post(userModel, cancellationToken);
        }

        return Created();
    }

    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Put(string id, UserModel updatedUserModel, CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetAsync(id, cancellationToken);

        if (user == null) return NotFound();

        updatedUserModel.Id = user.Id;

        var passwordHash = user.PasswordHash;

        if (updatedUserModel.Password != null)
        {
            passwordHash = BC.HashPassword(updatedUserModel.Password);
        }

        await _usersService.UpdateAsync(updatedUserModel.ToEntity(passwordHash), cancellationToken);

        var apiKey = await _cacheService.GetAsync<string?>(user.Id!, cancellationToken);

        if (apiKey != null)
        {
            await _cacheService.RemoveAsync(apiKey, cancellationToken);
        }

        return NoContent();
    }
}
