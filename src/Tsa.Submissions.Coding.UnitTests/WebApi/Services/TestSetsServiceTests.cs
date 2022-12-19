using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.ExtensionMethods;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Services;

public class TestSetsServiceTests
{
    private const string CollectionName = "test-sets";
    private const string DatabaseName = "submissions";

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Collection_Name_Should_Be_TestSets()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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
        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Assert
        Assert.Equal(CollectionName, testSetsService.CollectionName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Instantiate_Base_Class()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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
        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Assert
        Assert.NotNull(testSetsService);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void CreateAsync_Should_Insert_New_Entity()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSet = testSetsTestData.First(_ => (TestSetDataIssues) _[1] == TestSetDataIssues.None)[0] as TestSet;

        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();
        //mockedMongoCollection.Setup(_=>_.InsertOneAsync())

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        Func<TestSet, bool> validateTestSet = testSetToValidate =>
        {
            var inputsMatch = testSetToValidate.Inputs!.Count == testSet!.Inputs!.Count;
            var idsMatch = testSetToValidate.Id == testSet.Id;
            var isPublicMatch = testSetToValidate.IsPublic == testSet.IsPublic;
            var namesMatch = testSetToValidate.Name == testSet.Name;
            var problemsMatch = testSetToValidate.Problem == testSet.Problem;

            return inputsMatch && idsMatch && isPublicMatch && namesMatch && problemsMatch;
        };

        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        await testSetsService.CreateAsync(testSet!);

        // Assert
        mockedMongoCollection
            .Verify(_ =>
                _.InsertOneAsync(It.Is<TestSet>(c => validateTestSet(c)), null, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void ExistsAsync_Should_Return_True()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var expectedTestSet = testSetsTestData
            .Where(_ => (TestSetDataIssues) _[1] == TestSetDataIssues.None)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .Last();

        var testSets = new List<TestSet>
        {
            expectedTestSet
        };

        var mockedAsyncCursor = new Mock<IAsyncCursor<TestSet>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(testSets);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<TestSet>.Filter.Eq(_ => _.Id, expectedTestSet.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                It.Is<FilterDefinition<TestSet>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<TestSet, TestSet>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await testSetsService.ExistsAsync(expectedTestSet.Id!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetAsync_Should_Get_All_Entities()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSets = testSetsTestData
            .Where(_ => (TestSetDataIssues) _[1] == TestSetDataIssues.None)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .ToList();

        var mockedAsyncCursor = new Mock<IAsyncCursor<TestSet>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(testSets);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                Builders<TestSet>.Filter.Empty,
                It.IsAny<FindOptions<TestSet, TestSet>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await testSetsService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(testSets.Count, result.Count);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetAsync_Should_Get_Entity_By_Id()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var expectedTestSet = testSetsTestData
            .Where(_ => (TestSetDataIssues) _[1] == TestSetDataIssues.None)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .Last();

        var testSets = new List<TestSet>
        {
            expectedTestSet
        };

        var mockedAsyncCursor = new Mock<IAsyncCursor<TestSet>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(testSets);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<TestSet>.Filter.Eq(_ => _.Id, expectedTestSet.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                It.Is<FilterDefinition<TestSet>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<TestSet, TestSet>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await testSetsService.GetAsync(expectedTestSet.Id!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTestSet, result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void GetAsync_Should_Get_Entity_By_Ids()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var expectedTestSets = testSetsTestData
            .Where(_ => (TestSetDataIssues) _[1] == TestSetDataIssues.None)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .ToList();

        var ids = expectedTestSets.Select(_ => _.Id).Cast<string>().ToList();

        var mockedAsyncCursor = new Mock<IAsyncCursor<TestSet>>();
        mockedAsyncCursor.Setup(_ => _.Current).Returns(expectedTestSets);
        mockedAsyncCursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var filterDefinitionJson = Builders<TestSet>.Filter.In(_ => _.Id, ids).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();
        mockedMongoCollection
            .Setup(_ => _.FindAsync(
                It.Is<FilterDefinition<TestSet>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<TestSet, TestSet>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await testSetsService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTestSets.Count, result.Count);
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

        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await testSetsService.PingAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void Ping_Should_Return_True()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();
        var mockedMongoDatabase = new Mock<IMongoDatabase>();

        mockedMongoCollection
            .Setup(_ => _.Database)
            .Returns(mockedMongoDatabase.Object);

        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        var result = await testSetsService.PingAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void RemoveAsync_Should_Delete_Entity_By_Id()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var expectedTestSet = testSetsTestData
            .Where(_ => (TestSetDataIssues) _[1] == TestSetDataIssues.None)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .Last();

        var expectedFilterDefinitionJson = Builders<TestSet>.Filter.Eq(_ => _.Id, expectedTestSet.Id).RenderToJson();

        Func<FilterDefinition<TestSet>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        await testSetsService.RemoveAsync(expectedTestSet);

        // Assert
        mockedMongoCollection
            .Verify(_ => _.DeleteOneAsync(
                    It.Is<FilterDefinition<TestSet>>(fd => validateFilterDefinitionFunc(fd)),
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ServiceName_Name_Should_Be_TestSets()
    {
        // Arrange
        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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
        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Assert
        Assert.Equal("TestSets", testSetsService.ServiceName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async void UpdateAsync_Should_Replace_Entity_By_Id()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var expectedTestSet = testSetsTestData
            .Where(_ => (TestSetDataIssues) _[1] == TestSetDataIssues.None)
            .Select(_ => _[0])
            .Cast<TestSet>()
            .Last();

        var expectedFilterDefinitionJson = Builders<TestSet>.Filter.Eq(_ => _.Id, expectedTestSet.Id).RenderToJson();

        Func<FilterDefinition<TestSet>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        var mockedMongoCollection = new Mock<IMongoCollection<TestSet>>();

        var mockedMongoDatabase = new Mock<IMongoDatabase>();
        mockedMongoDatabase
            .Setup(_ => _.GetCollection<TestSet>(It.Is<string>(collectionName => collectionName == CollectionName), null))
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

        var testSetsService = new TestSetsService(mockedMongoClient.Object, mockedPointOfSalesOptions.Object);

        // Act
        await testSetsService.UpdateAsync(expectedTestSet, default);

        // Assert
        mockedMongoCollection
            .Verify(_ => _.ReplaceOneAsync(
                    It.Is<FilterDefinition<TestSet>>(fd => validateFilterDefinitionFunc(fd)),
                    expectedTestSet,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
