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

    public TeamsService(IMongoClient mongoClient, IOptions<SubmissionsDatabase> options) : base(
        mongoClient,
        options.Value.DatabaseName,
        MongoDbCollectionName) { }
}
