using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;
using Xunit;

namespace Tsa.Submissions.Coding.Tests.WebApi.Entities;

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
}
