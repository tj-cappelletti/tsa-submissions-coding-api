using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Options;
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
public class ProblemsServiceTest
{
    private const string CollectionName = "problems";
    private const string DatabaseName = "pos";

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Collection_Name_Should_Be_Problems()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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
        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Assert
        Assert.Equal("problems", problemsService.CollectionName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Instantiate_Base_Class()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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
        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Assert
        Assert.NotNull(problemsService);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void CreateAsync_Should_Insert_New_Entity()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)[0] as Problem;

        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        await problemsService.CreateAsync(problem!);

        // Assert
        mockedMongoCollection
            .Verify(_ =>
                _.InsertOneAsync(It.Is(problem, new ProblemEqualityComparer())!, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void ExistsAsync_Should_Return_True()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var expectedProblem = problemsTestData
            .Where(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)
            .Select(_ => _[0])
            .Cast<Problem>()
            .Last();

        var problems = new List<Problem>
        {
            expectedProblem
        };

        var mockedAsyncCursor = new Mock<IAsyncCursor<Problem>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(problems);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<Problem>.Filter.Eq(_ => _.Id, expectedProblem.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                It.Is<FilterDefinition<Problem>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<Problem, Problem>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await problemsService.ExistsAsync(expectedProblem.Id!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetAsync_Should_Get_All_Entities()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problems = problemsTestData
            .Where(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)
            .Select(_ => _[0])
            .Cast<Problem>()
            .ToList();

        var mockedAsyncCursor = new Mock<IAsyncCursor<Problem>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(problems);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                Builders<Problem>.Filter.Empty,
                It.IsAny<FindOptions<Problem, Problem>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await problemsService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(problems.Count, result.Count);
        Assert.Equal(problems, result, new ProblemEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetAsync_Should_Get_Entity_By_Id()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var expectedProblem = problemsTestData
            .Where(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)
            .Select(_ => _[0])
            .Cast<Problem>()
            .Last();

        var problems = new List<Problem>
        {
            expectedProblem
        };

        var mockedAsyncCursor = new Mock<IAsyncCursor<Problem>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(problems);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<Problem>.Filter.Eq(_ => _.Id, expectedProblem.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                It.Is<FilterDefinition<Problem>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<Problem, Problem>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await problemsService.GetAsync(expectedProblem.Id!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProblem, result, new ProblemEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetAsync_Should_Get_Entity_By_Ids()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var expectedProblems = problemsTestData
            .Where(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)
            .Select(_ => _[0])
            .Cast<Problem>()
            .ToList();

        var ids = expectedProblems.Select(_ => _.Id).Cast<string>().ToList();

        var mockedAsyncCursor = new Mock<IAsyncCursor<Problem>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(expectedProblems);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<Problem>.Filter.In(_ => _.Id, ids).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                It.Is<FilterDefinition<Problem>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<Problem, Problem>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await problemsService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProblems.Count, result.Count);
        Assert.Equal(expectedProblems, result, new ProblemEqualityComparer());
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

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await problemsService.PingAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Ping_Should_Return_True()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();
        var mockedMongoDatabase = new Mock<IMongoDatabase>();

        mockedMongoCollection
            .Setup(_ => _.Database)
            .Returns(mockedMongoDatabase.Object);

        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await problemsService.PingAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void RemoveAsync_Should_Delete_Entity_By_Id()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var expectedProblem = problemsTestData
            .Where(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)
            .Select(_ => _[0])
            .Cast<Problem>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Problem>.Filter.Eq(_ => _.Id, expectedProblem.Id).RenderToJson();

        Func<FilterDefinition<Problem>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        await problemsService.RemoveAsync(expectedProblem);

        // Assert
        mockedMongoCollection
            .Verify(_ => _.DeleteOneAsync(
                    It.Is<FilterDefinition<Problem>>(fd => validateFilterDefinitionFunc(fd)),
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ServiceName_Name_Should_Be_Problems()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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
        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Assert
        Assert.Equal("Problems", problemsService.ServiceName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void UpdateAsync_Should_Replace_Entity_By_Id()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var expectedProblem = problemsTestData
            .Where(_ => (ProblemDataIssues)_[1] == ProblemDataIssues.None)
            .Select(_ => _[0])
            .Cast<Problem>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Problem>.Filter.Eq(_ => _.Id, expectedProblem.Id).RenderToJson();

        Func<FilterDefinition<Problem>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        var mockedMongoCollection = new Mock<IMongoCollection<Problem>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<Problem>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        await problemsService.UpdateAsync(expectedProblem, default);

        // Assert
        mockedMongoCollection
            .Verify(_ => _.ReplaceOneAsync(
                    It.Is<FilterDefinition<Problem>>(fd => validateFilterDefinitionFunc(fd)),
                    expectedProblem,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
