using FluentValidation;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class TeamModelValidator : AbstractValidator<TeamModel>
{
    private const string ValidSchoolNumberRegEx = @"[\d]{4}";
    private const string ValidTeamNumberRegEx = @"9[\d]{2}";

    public TeamModelValidator()
    {
        RuleFor(team => team.SchoolNumber)
            .NotEmpty()
            .Matches(ValidSchoolNumberRegEx)
            .WithMessage("The School Number is required and must contain 4 digits");

        RuleFor(team => team.TeamNumber)
            .NotEmpty()
            .Matches(ValidTeamNumberRegEx)
            .WithMessage("The Team Number is required and must contain 3 digits and starts with a 9");

        RuleFor(team => team.Participants)
            .SetValidator(new ParticipantsPropertyValidator());
    }
}
