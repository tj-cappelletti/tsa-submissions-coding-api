using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class TeamsController : ControllerBase
{
    private readonly ITeamsService _teamsService;

    public TeamsController(ITeamsService teamsService)
    {
        _teamsService = teamsService;
    }

    /// <summary>
    ///     Deletes a team from database
    /// </summary>
    /// <param name="id">The ID of the team to be removed</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledges the team was successfully removed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The team to remove does not exist in the database</response>
    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var team = await _teamsService.GetAsync(id, cancellationToken);

        if (team == null) return NotFound();

        await _teamsService.RemoveAsync(team, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Fetches all the teams from the database
    /// </summary>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available teams returned</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TeamModel>))]
    public async Task<ActionResult<IList<TeamModel>>> Get(CancellationToken cancellationToken)
    {
        var teams = await _teamsService.GetAsync(cancellationToken);

        return User.IsInRole(SubmissionRoles.Participant)
            ? teams.Where(t => t.Participants.Any(p => p.ParticipantId == User.Identity!.Name)).ToList().ToModels()
            : teams.ToModels();
    }

    /// <summary>
    ///     Fetches a team from the database
    /// </summary>
    /// <param name="id">The ID of the team to get</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the requested team</response>
    /// <response code="404">The team does not exist in the database</response>
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamModel>> Get(string id, CancellationToken cancellationToken)
    {
        var team = await _teamsService.GetAsync(id, cancellationToken);

        if (team == null) return NotFound();

        if (User.IsInRole(SubmissionRoles.Participant) && team.Participants.All(p => p.ParticipantId != User.Identity!.Name))
        {
            // Return 404 to obfuscate the data
            return NotFound();
        }

        return team.ToModel();
    }

    /// <summary>
    ///     Creates a new team
    /// </summary>
    /// <param name="teamModel">The team to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the requested team</response>
    /// <response code="400">The team is not in a valid state and cannot be created</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<CreatedAtActionResult> Post(TeamModel teamModel, CancellationToken cancellationToken)
    {
        var team = teamModel.ToEntity();

        await _teamsService.CreateAsync(team, cancellationToken);

        teamModel.Id = team.Id;

        return CreatedAtAction(nameof(Get), new { id = team.Id }, teamModel);
    }

    /// <summary>
    ///     Updates the specified team
    /// </summary>
    /// <param name="id">The ID of the team to update</param>
    /// <param name="updatedTeamModel">The team that should replace the one in the database</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledgement that the team was updated</response>
    /// <response code="400">The team is not in a valid state and cannot be updated</response>
    /// <response code="404">The team requested to be updated could not be found</response>
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(string id, TeamModel updatedTeamModel, CancellationToken cancellationToken)
    {
        var team = await _teamsService.GetAsync(id, cancellationToken);

        if (team == null) return NotFound();

        updatedTeamModel.Id = team.Id;

        await _teamsService.UpdateAsync(updatedTeamModel.ToEntity(), cancellationToken);

        return NoContent();
    }
}
