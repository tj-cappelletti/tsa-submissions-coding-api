using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Exceptions;
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
    private readonly ITeamsService _teamsService;
    private readonly IUsersService _usersService;

    public UsersController(ICacheService cacheService, ITeamsService teamsService, IUsersService usersService)
    {
        _cacheService = cacheService;
        _teamsService = teamsService;
        _usersService = usersService;
    }

    public async Task CreateUser(User user, CancellationToken cancellationToken)
    {
        await _usersService.CreateAsync(user, cancellationToken);
    }

    private NotFoundObjectResult CreateUserNotFoundError(string id)
    {
        return NotFound(ApiErrorResponseModel.EntityNotFound(nameof(Entities.User), id));
    }

    /// <summary>
    ///     Deletes a user in the database
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledges the user was successfully removed</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The user to remove does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponseModel))]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetAsync(id, cancellationToken);

        if (user == null) return CreateUserNotFoundError(id);

        await _usersService.RemoveAsync(user, cancellationToken);

        return NoContent();
    }

    private async Task EnsureTeamExists(TeamModel teamModel, CancellationToken cancellationToken)
    {
        var teamExists = teamModel.Id != null
            ? await _teamsService.ExistsAsync(teamModel.Id, cancellationToken)
            : await _teamsService.ExistsAsync(teamModel.SchoolNumber, teamModel.TeamNumber, cancellationToken);

        if (teamExists) return;

        throw new RequiredEntityNotFoundException(nameof(Entities.User));
    }

    /// <summary>
    ///     Fetches all the users from the database
    /// </summary>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available users returned</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserModel>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var users = await _usersService.GetAsync(cancellationToken);

        return Ok(users.ToModels());
    }

    /// <summary>
    ///     Fetches a problem from the database
    /// </summary>
    /// <param name="id">The ID of the user to get</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the requested user</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The user does not exist in the database</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiErrorResponseModel))]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetAsync(id, cancellationToken);

        return user == null
            ? CreateUserNotFoundError(id)
            : Ok(user.ToModel());
    }

    /// <summary>
    ///     Creates a new user
    /// </summary>
    /// <param name="userModel">The user to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the created user</response>
    /// <response code="400">The user to create is not in a valid state and cannot be created</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post(UserModel userModel, CancellationToken cancellationToken = default)
    {
        // Team is required and is enforced in the model validation
        await EnsureTeamExists(userModel.Team!, cancellationToken);

        var passwordHash = BC.HashPassword(userModel.Password);

        var user = userModel.ToEntity(passwordHash);

        await CreateUser(user, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel());
    }

    /// <summary>
    ///     Creates multiple users in a batch operation
    /// </summary>
    /// <param name="userModels">An array of users to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the created users</response>
    /// <response code="400">The user to create is not in a valid state and cannot be created</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IList<UserModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post(UserModel[] userModels, CancellationToken cancellationToken = default)
    {
        var createdUserModels = new List<UserModel>(userModels.Length);

        foreach (var userModel in userModels)
        {
            var actionResult = await Post(userModel, cancellationToken);

            if (actionResult is not CreatedAtActionResult createdAtActionResult) throw new Exception("Failed to create user");

            createdUserModels.Add((UserModel)createdAtActionResult.Value!);
        }

        return Created("batch", createdUserModels);
    }

    /// <summary>
    ///     Updates the specified user
    /// </summary>
    /// <param name="id">The ID of the user to update</param>
    /// <param name="updatedUserModel">The user that should replace the one in the database</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledgement that the user was updated</response>
    /// <response code="400">The user to replace in the database with is not in a valid state and cannot be replaced</response>
    /// <response code="401">Authentication has failed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiErrorResponseModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Put(string id, UserModel updatedUserModel, CancellationToken cancellationToken = default)
    {
        // Team is required and is enforced in the model validation
        await EnsureTeamExists(updatedUserModel.Team!, cancellationToken);

        var user = await _usersService.GetAsync(id, cancellationToken);

        if (user == null) return CreateUserNotFoundError(id);

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
