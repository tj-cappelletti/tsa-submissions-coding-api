using System.Diagnostics.CodeAnalysis;
using FluentValidation.TestHelper;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Validators;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Validators;

[ExcludeFromCodeCoverage]
public class TeamModelValidatorTests
{
    [Theory]
    [ClassData(typeof(TeamsTestData))]
    [Trait("TestCategory", "UnitTest")]
    public void Should_Successfully_Validate_TeamModel(Team team, TeamDataIssues teamDataIssues)
    {
        // Arrange
        var teamModel = team.ToModel();

        var teamModelValidator = new TeamModelValidator();

        // Act

        var testValidationResult = teamModelValidator.TestValidate(teamModel);

        // Assert

        if (teamDataIssues.HasFlag(TeamDataIssues.InvalidParticipants))
        {
            testValidationResult.ShouldHaveValidationErrorFor(_ => _.Participants);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(_ => _.Participants);
        }

        if (teamDataIssues.HasFlag(TeamDataIssues.InvalidSchoolNumber))
        {
            testValidationResult.ShouldHaveValidationErrorFor(_ => _.SchoolNumber);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(_ => _.SchoolNumber);
        }

        if (teamDataIssues.HasFlag(TeamDataIssues.InvalidTeamNumber))
        {
            testValidationResult.ShouldHaveValidationErrorFor(_ => _.TeamNumber);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(_ => _.TeamNumber);
        }
    }
}
