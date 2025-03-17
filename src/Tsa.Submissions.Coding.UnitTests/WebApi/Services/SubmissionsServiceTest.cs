using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.ExtensionMethods;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Services;

[ExcludeFromCodeCoverage]
public class SubmissionsServiceTest
{
    private const string CollectionName = "submissions";
    private const string DatabaseName = "submissions";

    private readonly Mock<ILogger<SubmissionsService>> _mockedLogger = new();

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Collection_Name_Should_Be_Submissions()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.Equal("submissions", submissionsService.CollectionName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Instantiate_Base_Class()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.NotNull(submissionsService);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task CreateAsync_Should_Insert_New_Entity()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var submission =
            submissionsTestData.First(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)[0] as Submission;

        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await submissionsService.CreateAsync(submission!);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection =>
                mongoCollection.InsertOneAsync(It.Is(submission, new SubmissionEqualityComparer())!, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task ExistsAsync_Should_Return_True()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmission = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .Last();

        var submissions = new List<Submission>
        {
            expectedSubmission
        };

        var mockedAsyncCursor = new Mock<IAsyncCursor<Submission>>();
        mockedAsyncCursor.Setup(asyncCursor => asyncCursor.Current).Returns(submissions);
        mockedAsyncCursor
            .SetupSequence(asyncCursor => asyncCursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<Submission>.Filter.Eq(submission => submission.Id, expectedSubmission.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        mockedMongoCollection
            .Setup(mongoCollection => mongoCollection.FindAsync(
                It.Is<FilterDefinition<Submission>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<Submission, Submission>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.ExistsAsync(expectedSubmission.Id!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_All_Entities()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var submissions = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .ToList();

        var mockedAsyncCursor = new Mock<IAsyncCursor<Submission>>();
        mockedAsyncCursor.Setup(asyncCursor => asyncCursor.Current).Returns(submissions);
        mockedAsyncCursor
            .SetupSequence(asyncCursor => asyncCursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        mockedMongoCollection
            .Setup(mongoCollection => mongoCollection.FindAsync(
                Builders<Submission>.Filter.Empty,
                It.IsAny<FindOptions<Submission, Submission>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(submissions.Count, result.Count);
        Assert.Equal(submissions, result, new SubmissionEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_Entity_By_Id()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmission = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .Last();

        var submissions = new List<Submission>
        {
            expectedSubmission
        };

        var mockedAsyncCursor = new Mock<IAsyncCursor<Submission>>();
        mockedAsyncCursor.Setup(asyncCursor => asyncCursor.Current).Returns(submissions);
        mockedAsyncCursor
            .SetupSequence(asyncCursor => asyncCursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<Submission>.Filter.Eq(submission => submission.Id, expectedSubmission.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Submission>>();
        mockedMongoCollection
            .Setup(mongoCollection => mongoCollection.FindAsync(
                It.Is<FilterDefinition<Submission>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<Submission, Submission>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(mongoDatabase => mongoDatabase.GetCollection<Submission>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(mongoClient => mongoClient.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object)
            .Verifiable();

        var mockedSubmissionsDatabaseOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedSubmissionsDatabaseOptions
            .Setup(options => options.Value)
            .Returns(new SubmissionsDatabase
            {
                Name = DatabaseName
            });

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.GetAsync(expectedSubmission.Id!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSubmission, result, new SubmissionEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_Entity_By_Ids()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmissions = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .ToList();

        var ids = expectedSubmissions.Select(submission => submission.Id).Cast<string>().ToList();

        var mockedAsyncCursor = new Mock<IAsyncCursor<Submission>>();
        mockedAsyncCursor.Setup(asyncCursor => asyncCursor.Current).Returns(expectedSubmissions);
        mockedAsyncCursor
            .SetupSequence(asyncCursor => asyncCursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<Submission>.Filter.In(submission => submission.Id, ids).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        mockedMongoCollection
            .Setup(mongoCollection => mongoCollection.FindAsync(
                It.Is<FilterDefinition<Submission>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<Submission, Submission>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSubmissions.Count, result.Count);
        Assert.Equal(expectedSubmissions, result, new SubmissionEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Ping_Should_Return_False()
    {
        // Arrange
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        var expectedPingCommand = (Command<BsonDocument>)"{ping:1}";

        mockedMongoCollection
            .Setup(mongoCollection =>
                mongoCollection.Database.RunCommandAsync(It.Is<Command<BsonDocument>>(command => MockHelpers.ValidatePingCommand(expectedPingCommand, command)),
                    It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()))
            .Throws<Exception>()
            .Verifiable(Times.Once);

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.PingAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Ping_Should_Return_True()
    {
        // Arrange
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        var expectedPingCommand = (Command<BsonDocument>)"{ping:1}";

        mockedMongoCollection
            .Setup(mongoCollection =>
                mongoCollection.Database.RunCommandAsync(It.Is<Command<BsonDocument>>(command => MockHelpers.ValidatePingCommand(expectedPingCommand, command)),
                    It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()))
            .Verifiable(Times.Once);

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.PingAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RemoveAsync_Should_Delete_Entity_By_Id()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmission = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Submission>.Filter.Eq(submission => submission.Id, expectedSubmission.Id).RenderToJson();

        Func<FilterDefinition<Submission>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await submissionsService.RemoveAsync(expectedSubmission);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection => mongoCollection.DeleteOneAsync(
                    It.Is<FilterDefinition<Submission>>(fd => validateFilterDefinitionFunc(fd)),
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ServiceName_Name_Should_Be_Submissions()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.Equal("Submissions", submissionsService.ServiceName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task UpdateAsync_Should_Replace_Entity_By_Id()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmission = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Submission>.Filter.Eq(submission => submission.Id, expectedSubmission.Id).RenderToJson();

        Func<FilterDefinition<Submission>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.SetupMockedMongoObjects(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.SetupMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await submissionsService.UpdateAsync(expectedSubmission, default);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection => mongoCollection.ReplaceOneAsync(
                    It.Is<FilterDefinition<Submission>>(fd => validateFilterDefinitionFunc(fd)),
                    expectedSubmission,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
