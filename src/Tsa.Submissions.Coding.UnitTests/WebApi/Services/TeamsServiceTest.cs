using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.ExtensionMethods;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Services;

[ExcludeFromCodeCoverage]
public class TeamsServiceTest
{
    private const string CollectionName = "teams";
    private const string DatabaseName = "submissions";

    private readonly Mock<ILogger<TeamsService>> _mockedLogger = new();

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Collection_Name_Should_Be_Teams()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.Equal("teams", teamsService.CollectionName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Instantiate_Base_Class()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.NotNull(teamsService);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task CreateAsync_Should_Insert_New_Entity()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var team = teamsTestData.First(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)[0] as Team;

        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await teamsService.CreateAsync(team!);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection =>
                mongoCollection.InsertOneAsync(It.Is(team, new TeamEqualityComparer())!, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task ExistsAsync_By_Id_Should_Return_True()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeam = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .Last();

        var expectedTeams = new List<Team>
        {
            expectedTeam
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedTeams);

        var filterDefinitionJson = Builders<Team>.Filter.Eq(team => team.Id, expectedTeam.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await teamsService.ExistsAsync(expectedTeam.Id!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_By_SchoolNumber_And_TeamNumber_Should_Return_True()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeam = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .Last();

        var expectedTeams = new List<Team>
        {
            expectedTeam
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedTeams);

        var filterDefinitionJson = Builders<Team>.Filter.And(
                Builders<Team>.Filter.Eq(team => team.SchoolNumber, expectedTeam.SchoolNumber),
                Builders<Team>.Filter.Eq(team => team.TeamNumber, expectedTeam.TeamNumber))
            .RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await teamsService.ExistsAsync(expectedTeam.SchoolNumber, expectedTeam.TeamNumber);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_All_Entities()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeams = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .ToList();

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedTeams);

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await teamsService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(expectedTeams, result, new TeamEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_Entity_By_Id()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeam = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .Last();

        var expectedTeams = new List<Team>
        {
            expectedTeam
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedTeams);

        var filterDefinitionJson = Builders<Team>.Filter.Eq(team => team.Id, expectedTeam.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await teamsService.GetAsync(expectedTeam.Id!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTeam, result, new TeamEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_Entity_By_Ids()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeams = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .ToList();

        var ids = expectedTeams.Select(team => team.Id).Cast<string>().ToList();

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedTeams);

        var filterDefinitionJson = Builders<Team>.Filter.In(team => team.Id, ids).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await teamsService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTeams, result, new TeamEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Return_Entity_By_SchoolNumber_TeamNumber()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeam = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .Last();

        var expectedTeams = new List<Team>
        {
            expectedTeam
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedTeams);

        var filterDefinitionJson = Builders<Team>.Filter.And(
            Builders<Team>.Filter.Eq(team => team.SchoolNumber, expectedTeam.SchoolNumber),
            Builders<Team>.Filter.Eq(team => team.TeamNumber, expectedTeam.TeamNumber)).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        mockedMongoCollection
            .Setup(mongoCollection => mongoCollection.FindAsync(
                It.Is<FilterDefinition<Team>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<Team, Team>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await teamsService.GetAsync(expectedTeam.SchoolNumber, expectedTeam.TeamNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTeam, result, new TeamEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Ping_Should_Return_False()
    {
        // Arrange
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        var expectedPingCommand = (Command<BsonDocument>)"{ping:1}";

        MockHelpers.SetupMockedMongoCollectionRunCommandAsyncThrowsException(mockedMongoCollection, expectedPingCommand);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await teamsService.PingAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Ping_Should_Return_True()
    {
        // Arrange
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        var expectedPingCommand = (Command<BsonDocument>)"{ping:1}";

        MockHelpers.SetupMockedMongoCollectionRunCommandAsync(mockedMongoCollection, expectedPingCommand);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await teamsService.PingAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RemoveAsync_Should_Delete_Entity_By_Id()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeam = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Team>.Filter.Eq(team => team.Id, expectedTeam.Id).RenderToJson();

        Func<FilterDefinition<Team>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await teamsService.RemoveAsync(expectedTeam);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection => mongoCollection.DeleteOneAsync(
                    It.Is<FilterDefinition<Team>>(fd => validateFilterDefinitionFunc(fd)),
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ServiceName_Name_Should_Be_Teams()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.Equal("Teams", teamsService.ServiceName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task UpdateAsync_Should_Replace_Entity_By_Id()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeam = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Team>.Filter.Eq(team => team.Id, expectedTeam.Id).RenderToJson();

        Func<FilterDefinition<Team>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Team>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await teamsService.UpdateAsync(expectedTeam, default);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection => mongoCollection.ReplaceOneAsync(
                    It.Is<FilterDefinition<Team>>(fd => validateFilterDefinitionFunc(fd)),
                    expectedTeam,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
