using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class TeamsService : MongoDbService<Team>, ITeamsService
{
    public const string MongoDbCollectionName = "teams";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "Teams";

    public TeamsService(IMongoClient mongoClient, IOptions<SubmissionsDatabase> options, ILogger<TeamsService> logger) : base(
        mongoClient,
        options.Value.Name!,
        MongoDbCollectionName,
        logger)
    { }

    public async Task<bool> ExistsAsync(string? schoolNumber, string? teamNumber, CancellationToken cancellationToken = default)
    {
        var filterDefinition =
            Builders<Team>.Filter.And(
                Builders<Team>.Filter.Eq(team => team.SchoolNumber, schoolNumber),
                Builders<Team>.Filter.Eq(team => team.TeamNumber, teamNumber));

        var cursor = await EntityCollection.FindAsync(filterDefinition, cancellationToken: cancellationToken);

        return await cursor.AnyAsync(cancellationToken);
    }
}
