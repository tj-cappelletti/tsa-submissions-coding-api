using System.Threading;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public interface ITeamsService : IMongoEntityService<Team>, IPingableService
{
    Task<bool> ExistsAsync(string? schoolNumber, string? teamNumber, CancellationToken cancellationToken = default);
}
