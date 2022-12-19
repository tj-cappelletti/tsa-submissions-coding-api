using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class TestSetsService : MongoDbService<TestSet>, ITestSetsService
{
    public const string MongoDbCollectionName = "test-sets";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "TestSets";

    public TestSetsService(IMongoClient mongoClient, IOptions<SubmissionsDatabase> options) : base(
        mongoClient,
        options.Value.DatabaseName,
        MongoDbCollectionName)
    { }
}
