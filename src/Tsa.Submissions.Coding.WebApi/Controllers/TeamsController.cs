using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeamsController : ControllerBase
{
    private readonly ITeamsService _teamsService;

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

    [HttpGet]
    public async Task<IEnumerable<Team>> Get()
    {
        return await _teamsService.GetAsync();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Team>> Get(string id)
    {
        var team = await _teamsService.GetAsync(id);

        return team == null
            ? NotFound()
            : team;
    }

    [HttpPost]
    public async Task<ActionResult<Team>> Post(Team newTeam)
    {
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
