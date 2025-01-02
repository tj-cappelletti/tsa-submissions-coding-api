using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Models;

[ExcludeFromCodeCoverage]
public class ModelExtensions
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntities_For_ParticipantModels_Should_Return_Participants()
    {
        // Arrange
        var participantModels = new List<ParticipantModel>
        {
            new() { ParticipantNumber = "001", SchoolNumber = "1234" },
            new() { ParticipantNumber = "002", SchoolNumber = "1234" }
        };

        // Act
        var participants = participantModels.ToEntities();

        // Assert
        Assert.Equal(participantModels.Count, participants.Count);

        foreach (var participant in participants)
        {
            Assert.Contains(participantModels, _ => _.ParticipantId == participant.ParticipantId);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntities_For_TestSetInputModels_Should_Return_Null_When_Inputs_Is_Null()
    {
        // Arrange
        var testSetModel = new TestSetModel();

        // Act
        var testSetInputs = testSetModel.Inputs.ToEntities();

        // Assert
        Assert.Null(testSetInputs);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_ParticipantModel_Should_Return_Participant()
    {
        // Arrange
        var participantModel = new ParticipantModel
        {
            ParticipantNumber = "123",
            SchoolNumber = "1234"
        };

        // Act
        var participant = participantModel.ToEntity();

        // Assert
        Assert.Equal(participantModel.ParticipantNumber, participant.ParticipantNumber);
        Assert.Equal(participantModel.SchoolNumber, participant.SchoolNumber);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_ProblemModel_Should_Return_Problem()
    {
        // Arrange
        var problemModel = new ProblemModel
        {
            Description = "This is the description",
            Id = "This is the ID",
            IsActive = true,
            Title = "This is the title"
        };

        // Act
        var problem = problemModel.ToEntity();

        // Assert
        Assert.Equal(problemModel.Description, problem.Description);
        Assert.Equal(problemModel.Id, problem.Id);
        Assert.Equal(problemModel.IsActive, problem.IsActive);
        Assert.Equal(problemModel.Title, problem.Title);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_SubmissionModel_Should_Return_Submission()
    {
        // Arrange
        var submissionModel = new SubmissionModel
        {
            Id = "000000000000000000000000",
            IsFinalSubmission = true,
            Language = "csharp",
            ProblemId = "00000000000000000000000A",
            Solution = "The solution",
            SubmittedOn = DateTime.Now.AddHours(-5),
            TeamId = "00000000000000000000000B",
            TestSetResults =
            [
                new()
                {
                    Passed = true,
                    RunDuration = new TimeSpan(0, 0, 5, 0),
                    TestSetId = "000000000000000000000010"
                },
                new()
                {
                    Passed = true,
                    RunDuration = new TimeSpan(0, 0, 5, 0),
                    TestSetId = "000000000000000000000011"
                }
            ]
        };

        // Act
        var submission = submissionModel.ToEntity();

        Assert.Equal(submissionModel.Id, submission.Id);
        Assert.Equal(submissionModel.IsFinalSubmission, submission.IsFinalSubmission);
        Assert.Equal(submissionModel.Language, submission.Language);
        Assert.Equal(submissionModel.ProblemId, submission.Problem?.Id.AsString);
        Assert.Equal(submissionModel.Solution, submission.Solution);
        Assert.Equal(submissionModel.SubmittedOn, submission.SubmittedOn);
        Assert.Equal(submissionModel.TeamId, submission.Team?.Id.AsString);
        Assert.NotNull(submission.TestSetResults);
        Assert.Equal(submissionModel.TestSetResults.Count, submission.TestSetResults.Count);

        foreach (var testSetResultModel in submissionModel.TestSetResults)
        {
            var testSetResult = submission.TestSetResults.SingleOrDefault(_ => _.TestSet?.Id.AsString == testSetResultModel.TestSetId);

            Assert.NotNull(testSetResult);
            Assert.Equal(testSetResultModel.Passed, testSetResult.Passed);
            Assert.Equal(testSetResultModel.RunDuration, testSetResult.RunDuration);
            Assert.Equal(testSetResultModel.TestSetId, testSetResult.TestSet?.Id.AsString);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_TeamModel_Should_Return_Team()
    {
        // Arrange
        var teamModel = new TeamModel
        {
            CompetitionLevel = "MiddleSchool",
            Id = "This is an ID",
            Participants =
            [
                new ParticipantModel
                {
                    ParticipantNumber = "001",
                    SchoolNumber = "1234"
                },
                new ParticipantModel
                {
                    ParticipantNumber = "002",
                    SchoolNumber = "1234"
                }
            ],
            SchoolNumber = "1234",
            TeamNumber = "901"
        };

        var expectedTeam = new Team
        {
            CompetitionLevel = Enum.Parse<CompetitionLevel>(teamModel.CompetitionLevel),
            Id = teamModel.Id,
            Participants =
            [
                new Participant
                {
                    ParticipantNumber = teamModel.Participants[0].ParticipantNumber,
                    SchoolNumber = teamModel.Participants[0].SchoolNumber
                },
                new Participant
                {
                    ParticipantNumber = teamModel.Participants[1].ParticipantNumber,
                    SchoolNumber = teamModel.Participants[1].SchoolNumber
                }
            ],
            SchoolNumber = teamModel.SchoolNumber,
            TeamNumber = teamModel.TeamNumber
        };

        // Act
        var team = teamModel.ToEntity();

        // Assert
        Assert.Equal(teamModel.CompetitionLevel, team.CompetitionLevel.ToString(), new StringEqualityComparer());
        Assert.Equal(teamModel.Id, team.Id);
        Assert.Equal(teamModel.SchoolNumber, team.SchoolNumber);
        Assert.Equal(teamModel.TeamNumber, team.TeamNumber);

        Assert.Equal(teamModel.Participants.Count, team.Participants.Count);

        foreach (var participant in team.Participants)
        {
            Assert.Contains(teamModel.Participants, participantModel => participantModel.ParticipantId == participant.ParticipantId);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_TestSetInputModel_Should_Return_TestSetInput()
    {
        // Arrange
        var testSetInputModel = new TestSetValueModel
        {
            DataType = "Data Type",
            Index = 9999,
            IsArray = true,
            ValueAsJson = "ValueAsJson"
        };

        // Act
        var testSetInput = testSetInputModel.ToEntity();

        // Assert
        Assert.Equal(testSetInputModel.DataType, testSetInput.DataType);
        Assert.Equal(testSetInputModel.Index, testSetInput.Index);
        Assert.Equal(testSetInputModel.IsArray, testSetInput.IsArray);
        Assert.Equal(testSetInputModel.ValueAsJson, testSetInput.ValueAsJson);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_TestSetModel_Should_Return_TestSet()
    {
        // Arrange
        var testSetModel = new TestSetModel
        {
            Id = "This is an ID",
            Inputs =
            [
                new()
                {
                    DataType = "Data Type #1",
                    Index = 1,
                    IsArray = true,
                    ValueAsJson = "ValueAsJson #1"
                },
                new()
                {
                    DataType = "Data Type #2",
                    Index = 2,
                    IsArray = false,
                    ValueAsJson = "ValueAsJson #2"
                },
                new()
                {
                    DataType = "Data Type #3",
                    Index = 3,
                    IsArray = true,
                    ValueAsJson = "ValueAsJson #3"
                }
            ],
            IsPublic = true,
            Name = "Test Set #1",
            ProblemId = "000000000000000000000000"
        };

        // Act
        var testSet = testSetModel.ToEntity();

        // Assert
        Assert.Equal(testSetModel.Id, testSet.Id);
        Assert.NotNull(testSet.Inputs);
        Assert.NotEmpty(testSet.Inputs);

        foreach (var testSetInputModel in testSetModel.Inputs)
        {
            var testSetInput = testSet.Inputs.SingleOrDefault(_ => _.Index == testSetInputModel.Index);

            Assert.NotNull(testSetInput);
            Assert.Equal(testSetInputModel.DataType, testSetInput.DataType);
            Assert.Equal(testSetInputModel.IsArray, testSetInput.IsArray);
            Assert.Equal(testSetInputModel.ValueAsJson, testSetInput.ValueAsJson);
        }

        Assert.Equal(testSetModel.Name, testSet.Name);
        Assert.Equal(testSetModel.ProblemId, testSet.Problem?.Id.AsString);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_TestSetResultModel_Should_Return_TestSetResult()
    {
        // Arrange
        var testSetResultModel = new TestSetResultModel
        {
            Passed = true,
            RunDuration = new TimeSpan(0, 0, 5, 0),
            TestSetId = "000000000000000000000010"
        };

        // Act
        var testSetResult = testSetResultModel.ToEntity();

        // Assert
        Assert.Equal(testSetResultModel.Passed, testSetResult.Passed);
        Assert.Equal(testSetResultModel.RunDuration, testSetResult.RunDuration);
        Assert.Equal(testSetResultModel.TestSetId, testSetResult.TestSet?.Id.AsString);
    }
}
