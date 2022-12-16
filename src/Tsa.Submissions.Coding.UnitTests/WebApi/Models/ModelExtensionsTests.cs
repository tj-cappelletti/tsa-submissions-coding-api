using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public void ToEntity_For_TeamModel_Should_Return_Team()
    {
        // Arrange
        var teamModel = new TeamModel
        {
            Id = "This is an ID",
            Participants = new List<ParticipantModel>
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
        var team = teamModel.ToEntity();

        // Assert
        Assert.Equal(teamModel.Id, team.Id);
        Assert.Equal(teamModel.SchoolNumber, team.SchoolNumber);
        Assert.Equal(teamModel.TeamNumber, team.TeamNumber);

        Assert.Equal(teamModel.Participants.Count, team.Participants.Count);

        foreach (var participant in team.Participants)
        {
            Assert.Contains(teamModel.Participants, _ => _.ParticipantId == participant.ParticipantId);
        }
    }
}
