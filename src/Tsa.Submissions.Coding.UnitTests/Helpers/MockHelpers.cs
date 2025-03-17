using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

internal static class MockHelpers
{
    public static (Mock<IMongoCollection<Submission>>, Mock<IMongoClient>) SetupMockedMongoObjects(string databaseName, string collectionName)
    {
        var mockedMongoDatabase = new Mock<IMongoDatabase>();

        var mockedMongoCollection = new Mock<IMongoCollection<Submission>>();
        mockedMongoCollection
            .Setup(mongoCollection => mongoCollection.Database)
            .Returns(mockedMongoDatabase.Object);

        mockedMongoDatabase
            .Setup(mongoDatabase => mongoDatabase.GetCollection<Submission>(It.Is<string>(name => name == collectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(mongoClient => mongoClient.GetDatabase(It.Is<string>(name => name == databaseName), null))
            .Returns(mockedMongoDatabase.Object);

        return (mockedMongoCollection, mockedMongoClient);
    }

    public static Mock<IOptions<SubmissionsDatabase>> SetupMockedSubmissionsDatabaseOptions(string databaseName)
    {
        var mockedSubmissionsDatabaseOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedSubmissionsDatabaseOptions
            .Setup(options => options.Value)
            .Returns(new SubmissionsDatabase
            {
                Name = databaseName
            });

        return mockedSubmissionsDatabaseOptions;
    }

    public static bool ValidatePingCommand(Command<BsonDocument> expectedCommand, Command<BsonDocument> actualCommand)
    {
        var expectedCommandJson = expectedCommand.ToJson();
        var actualCommandJson = actualCommand.ToJson();

        return expectedCommandJson == actualCommandJson;
    }
}
