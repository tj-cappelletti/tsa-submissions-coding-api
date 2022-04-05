using System.Collections.Generic;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface ITeamsService
{
    Task CreateAsync(Team newTeam);

    Task<List<Team>> GetAsync();

    Task<Team> GetAsync(string id);

    Task RemoveAsync(string id);

    Task UpdateAsync(string id, Team updatedTeam);

    Task<bool> Ping();
}
