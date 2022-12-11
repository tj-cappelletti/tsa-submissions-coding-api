using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class ProblemsService: MongoDbService<Problem>, IProblemsService
{
    public const string MongoDbCollectionName = "problems";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "Problems";

    public ProblemsService(IMongoClient mongoClient, IOptions<SubmissionsDatabase> options) : base(
        mongoClient,
        options.Value.DatabaseName,
        MongoDbCollectionName)
    { }
}
