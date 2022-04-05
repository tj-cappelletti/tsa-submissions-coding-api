using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusController : ControllerBase
{
    private readonly ITeamsService _teamsService;

    public StatusController(ITeamsService teamsService)
    {
        _teamsService = teamsService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var status = new Status
        {
            TeamsServiceIsAlive = await _teamsService.Ping()
        };

        return status.IsHealthy
            ? Ok(status)
            : StatusCode(StatusCodes.Status500InternalServerError, status);
    }
}
