using System;
using FluentValidation;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class TeamModelValidator : AbstractValidator<TeamModel>
{
    private const string ValidSchoolNumberRegEx = @"[\d]{4}";
    private const string ValidTeamNumberRegEx = @"9[\d]{2}";

    public TeamModelValidator()
    {
        RuleFor(team => team.CompetitionLevel)
            .NotEmpty()
            .IsEnumName(typeof(CompetitionLevel), false)
            .WithMessage("The competition level must be either 'MiddleSchool' or 'HighSchool'.")
            .DependentRules(() =>
            {
                When(team => !string.IsNullOrWhiteSpace(team.CompetitionLevel) && !string.IsNullOrWhiteSpace(team.SchoolNumber), () =>
                {
                    RuleFor(team => new { team.CompetitionLevel, team.SchoolNumber })
                        // Null suppression is used here to avoid a warning about the nullable property being null
                        // The When method should prevent this from being an issue
                        .Must(team => ValidateCompetitionAndSchoolNumberLevel(team.CompetitionLevel!, team.SchoolNumber!))
                        .WithMessage(
                            "The School Number must start with a 1 for Middle School level competitions and 2 for High School level competition");
                });
            });

        RuleFor(team => team.SchoolNumber)
            .NotEmpty()
            .Matches(ValidSchoolNumberRegEx)
            .WithMessage("The School Number is required and must contain 4 digits");

        RuleFor(team => team.TeamNumber)
            .NotEmpty()
            .Matches(ValidTeamNumberRegEx)
            .WithMessage("The Team Number is required and must contain 3 digits and starts with a 9");

        RuleFor(team => team.Participants)
            .SetValidator(new ParticipantsPropertyValidator())
            .WithMessage("The participant(s) are not valid");
    }

    private static bool ValidateCompetitionAndSchoolNumberLevel(string competitionLevel, string schoolNumber)
    {
        var competitionLevelEnum = Enum.Parse<CompetitionLevel>(competitionLevel);

        return competitionLevelEnum switch
        {
            CompetitionLevel.MiddleSchool => schoolNumber.StartsWith("1"),
            CompetitionLevel.HighSchool => schoolNumber.StartsWith("2"),
            _ => false
        };
    }
}
