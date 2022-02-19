using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeamsController : ControllerBase
{
    private readonly ITeamsService _teamsService;
    private const string ValidParticipantIdRegEx = @"[\d]{4}-[0-8][\d]{2}";
    private const string ValidTeamIdRegEx = @"[\d]{4}-9[\d]{2}";

    public TeamsController(ITeamsService teamsService)
    {
        _teamsService = teamsService;
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var team = await _teamsService.GetAsync(id);

        if (team == null) return NotFound();

        await _teamsService.RemoveAsync(id);

        return NoContent();
    }

    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpGet]
    public async Task<IEnumerable<Team>> Get()
    {
        return await _teamsService.GetAsync();
    }

    [Authorize(Roles = SubmissionRoles.All)]
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Team>> Get(string id)
    {
        var team = await _teamsService.GetAsync(id);

        if (User.IsInRole(SubmissionRoles.Participant) &&
            (team == null || team.Participants.All(p => p.ParticipantId != User!.Identity!.Name)))
            return Unauthorized();

        return team == null
            ? NotFound()
            : team;
    }

    private static bool IsTeamValid(Team team)
    {
        if (!Regex.IsMatch(team.TeamId, ValidTeamIdRegEx)) return false;

        if (!team.Participants.Any()) return true;

        if (team.Participants.Count > 2) return false;

        if (team.Participants.Any(p => !Regex.IsMatch(p.ParticipantId, ValidParticipantIdRegEx))) return false;

        if (team.Participants.Any(p => p.SchoolNumber != team.SchoolNumber)) return false;

        if (team.Participants.Select(p => p.ParticipantId).Distinct().Count() != team.Participants.Count) return false;

        return true;
    }

    [Authorize(Roles = SubmissionRoles.Judge)]
    [HttpPost]
    public async Task<ActionResult<Team>> Post(Team newTeam)
    {
        if (!IsTeamValid(newTeam)) return BadRequest();

        await _teamsService.CreateAsync(newTeam);

        return CreatedAtAction(nameof(Get), new { id = newTeam.Id }, newTeam);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Put(string id, Team updatedTeam)
    {
        var team = await _teamsService.GetAsync(id);

        if (team == null) return NotFound();

        updatedTeam.Id = team.Id;

        await _teamsService.UpdateAsync(id, updatedTeam);

        return NoContent();
    }
}
