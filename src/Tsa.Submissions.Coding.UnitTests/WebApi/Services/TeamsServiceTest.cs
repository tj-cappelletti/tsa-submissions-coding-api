using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Tsa.Submissions.Coding.Tests.Data;
using Tsa.Submissions.Coding.Tests.ExtensionMethods;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.Tests.WebApi.Services;

[ExcludeFromCodeCoverage]
public class TeamsServiceTest
{
    private const string CollectionName = "teams";
    private const string DatabaseName = "pos";

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Collection_Name_Should_Be_Teams()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object);

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        // Act
        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Assert
        Assert.Equal("teams", teamsService.CollectionName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Instantiate_Base_Class()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object);

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        // Act
        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Assert
        Assert.NotNull(teamsService);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void CreateAsync_Should_Insert_New_Entity()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var team = teamsTestData.First(_ => (TeamDataIssues)_[1] == TeamDataIssues.None)[0] as Team;

        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();
        //mockedMongoCollection.Setup(_=>_.InsertOneAsync())

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object)
            .Verifiable();

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object)
            .Verifiable();

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        Func<Team, bool> validateTeam = teamToValidate =>
        {
            var idsMatch = teamToValidate.Id == team!.Id;
            var schoolNumbersMatch = teamToValidate.SchoolNumber == team.SchoolNumber;
            var teamNumbersMatch = teamToValidate.TeamNumber == team.TeamNumber;

            var participantsMatch = teamToValidate.Participants.Count == team.Participants.Count &&
                                    teamToValidate.Participants[0].ParticipantNumber == team.Participants[0].ParticipantNumber &&
                                    teamToValidate.Participants[0].SchoolNumber == team.Participants[0].SchoolNumber;

            return idsMatch && participantsMatch && schoolNumbersMatch && teamNumbersMatch;
        };

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        await teamsService.CreateAsync(team!);

        // Assert
        mockedMongoCollection
            .Verify(_ =>
                _.InsertOneAsync(It.Is<Team>(c => validateTeam(c)), null, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void ExistsAsync_Should_Return_True()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeam = teamsTestData
            .Where(_ => (TeamDataIssues)_[1] == TeamDataIssues.None)
            .Select(_ => _[0])
            .Cast<Team>()
            .Last();

        var teams = new List<Team>
        {
            expectedTeam
        };

        var mockedAsyncCursor = new Mock<IAsyncCursor<Team>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(teams);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<Team>.Filter.Eq(_ => _.Id, expectedTeam.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                It.Is<FilterDefinition<Team>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<Team, Team>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object)
            .Verifiable();

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await teamsService.ExistsAsync(expectedTeam.Id!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetAsync_Should_Get_All_Entities()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var teams = teamsTestData
            .Where(_ => (TeamDataIssues)_[1] == TeamDataIssues.None)
            .Select(_ => _[0])
            .Cast<Team>()
            .ToList();

        var mockedAsyncCursor = new Mock<IAsyncCursor<Team>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(teams);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                Builders<Team>.Filter.Empty,
                It.IsAny<FindOptions<Team, Team>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object)
            .Verifiable();

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await teamsService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(teams.Count, result.Count);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetAsync_Should_Get_Entity_By_Id()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeam = teamsTestData
            .Where(_ => (TeamDataIssues)_[1] == TeamDataIssues.None)
            .Select(_ => _[0])
            .Cast<Team>()
            .Last();

        var teams = new List<Team>
        {
            expectedTeam
        };

        var mockedAsyncCursor = new Mock<IAsyncCursor<Team>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(teams);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<Team>.Filter.Eq(_ => _.Id, expectedTeam.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                It.Is<FilterDefinition<Team>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<Team, Team>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object)
            .Verifiable();

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await teamsService.GetAsync(expectedTeam.Id!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTeam, result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetAsync_Should_Get_Entity_By_Ids()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeams = teamsTestData
            .Where(_ => (TeamDataIssues)_[1] == TeamDataIssues.None)
            .Select(_ => _[0])
            .Cast<Team>()
            .ToList();

        var ids = expectedTeams.Select(_ => _.Id).Cast<string>().ToList();

        var mockedAsyncCursor = new Mock<IAsyncCursor<Team>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(expectedTeams);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<Team>.Filter.In(_ => _.Id, ids).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                It.Is<FilterDefinition<Team>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<Team, Team>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object)
            .Verifiable();

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await teamsService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTeams.Count, result.Count);
    }


    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Ping_Should_Return_False()
    {
        // Arrange
        var mockedMongoDatabase = new Mock<IMongoDatabase>();

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object)
            .Verifiable();

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await teamsService.PingAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Ping_Should_Return_True()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();
        var mockedMongoDatabase = new Mock<IMongoDatabase>();

        mockedMongoCollection
            .Setup(_ => _.Database)
            .Returns(mockedMongoDatabase.Object);

        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object)
            .Verifiable();

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await teamsService.PingAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void RemoveAsync_Should_Delete_Entity_By_Id()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeam = teamsTestData
            .Where(_ => (TeamDataIssues)_[1] == TeamDataIssues.None)
            .Select(_ => _[0])
            .Cast<Team>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Team>.Filter.Eq(_ => _.Id, expectedTeam.Id).RenderToJson();

        Func<FilterDefinition<Team>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object)
            .Verifiable();

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        await teamsService.RemoveAsync(expectedTeam);

        // Assert
        mockedMongoCollection
            .Verify(_ => _.DeleteOneAsync(
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
        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object);

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        // Act
        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Assert
        Assert.Equal("Teams", teamsService.ServiceName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void UpdateAsync_Should_Replace_Entity_By_Id()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var expectedTeam = teamsTestData
            .Where(_ => (TeamDataIssues)_[1] == TeamDataIssues.None)
            .Select(_ => _[0])
            .Cast<Team>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Team>.Filter.Eq(_ => _.Id, expectedTeam.Id).RenderToJson();

        Func<FilterDefinition<Team>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        var mockedMongoCollection = new Mock<IMongoCollection<Team>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Team>(It.Is<string>(collectionName => collectionName == CollectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(_ => _.GetDatabase(It.Is<string>(databaseName => databaseName == DatabaseName), null))
            .Returns(mockedMongoDatabase.Object)
            .Verifiable();

        var mockedPointOfSalesOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedPointOfSalesOptions
            .Setup(_ => _.Value)
            .Returns(new SubmissionsDatabase
            {
                DatabaseName = DatabaseName
            });

        var teamsService = new TeamsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        await teamsService.UpdateAsync(expectedTeam, default);

        // Assert
        mockedMongoCollection
            .Verify(_ => _.ReplaceOneAsync(
                    It.Is<FilterDefinition<Team>>(fd => validateFilterDefinitionFunc(fd)),
                    expectedTeam,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
