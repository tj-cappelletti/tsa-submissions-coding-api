using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Entities;

[ExcludeFromCodeCoverage]
public class EntityExtensions
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Participant_Should_Return_ParticipantModel()
    {
        // Arrange
        var participant = new Participant
        {
            ParticipantNumber = "123",
            SchoolNumber = "1234"
        };

        // Act
        var participantModel = participant.ToModel();

        // Assert
        Assert.Equal(participant.ParticipantNumber, participantModel.ParticipantNumber);
        Assert.Equal(participant.SchoolNumber, participantModel.SchoolNumber);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Problem_Should_Return_ProblemModel()
    {
        // Arrange
        var problem = new Problem
        {
            Description = "This is the description",
            Id = "This is the ID",
            IsActive = true,
            Title = "This is the title"
        };

        // Act
        var problemModel = problem.ToModel();

        // Assert
        Assert.Equal(problem.Description, problemModel.Description);
        Assert.Equal(problem.Id, problemModel.Id);
        Assert.Equal(problem.IsActive, problemModel.IsActive);
        Assert.Equal(problem.Title, problemModel.Title);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Team_Should_Return_TeamModel()
    {
        // Arrange
        var team = new Team
        {
            Id = "This is an ID",
            Participants = new List<Participant>
            {
                new()
                {
                    ParticipantNumber = "001",
                    SchoolNumber = "1234"
                },
                new()
                {
                    ParticipantNumber = "002",
                    SchoolNumber = "1234"
                }
            },
            SchoolNumber = "1234",
            TeamNumber = "901"
        };

        // Act
        var teamModel = team.ToModel();

        // Assert
        Assert.Equal(team.Id, teamModel.Id);
        Assert.Equal(team.SchoolNumber, teamModel.SchoolNumber);
        Assert.Equal(team.TeamNumber, teamModel.TeamNumber);

        Assert.Equal(team.Participants.Count, teamModel.Participants.Count);

        foreach (var teamModelParticipant in teamModel.Participants)
        {
            Assert.Contains(team.Participants, _ => _.ParticipantId == teamModelParticipant.ParticipantId);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_TestSet_Should_Return_TestSetModel()
    {
        // Arrange
        var testSet = new TestSet
        {
            Id = "This is an ID",
            Inputs = new List<TestSetInput>
            {
                new()
                {
                    DataType = "Data Type #1",
                    Index = 1,
                    Value = "Value #1"
                },
                new()
                {
                    DataType = "Data Type #2",
                    Index = 2,
                    Value = "Value #2"
                },
                new()
                {
                    DataType = "Data Type #3",
                    Index = 3,
                    Value = "Value #3"
                }
            },
            IsPublic = true,
            Name = "Test Set #1",
            Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000000")
        };

        // Act
        var testSetModel = testSet.ToModel();

        // Assert
        Assert.Equal(testSet.Id, testSetModel.Id);
        Assert.NotNull(testSetModel.Inputs);
        Assert.NotEmpty(testSetModel.Inputs);

        foreach (var testSetInput in testSet.Inputs)
        {
            var testSetInputModel = testSetModel.Inputs.SingleOrDefault(_ => _.Index == testSetInput.Index);

            Assert.NotNull(testSetInputModel);
            Assert.Equal(testSetInput.DataType, testSetInputModel.DataType);
            Assert.Equal(testSetInput.Value, testSetInputModel.Value);
        }

        Assert.Equal(testSet.Name, testSetModel.Name);
        Assert.Equal(testSet.Problem?.Id.AsString, testSetModel.ProblemId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_TestSetInput_Should_Return_TestSetInputModel()
    {
        // Arrange
        var testSetInput = new TestSetInput
        {
            DataType = "Data Type",
            Index = 9999,
            Value = "Value"
        };

        // Act
        var testSetInputModel = testSetInput.ToModel();

        // Assert
        Assert.Equal(testSetInput.DataType, testSetInputModel.DataType);
        Assert.Equal(testSetInput.Index, testSetInputModel.Index);
        Assert.Equal(testSetInput.Value, testSetInputModel.Value);
    }
}
