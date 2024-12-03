using System;
using System.Linq;
using FluentValidation;
using Tsa.Submissions.Coding.WebApi.Authorization;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class UserModelValidator : AbstractValidator<UserModel>
{
    public UserModelValidator()
    {
        RuleFor(user => user.Password)
            .Custom((password, context) =>
            {
                if (password == null) return;

                if (password.Length < 8)
                {
                    context.AddFailure("The password must be at least 8 characters long.");
                }

                if (!password.Any(char.IsUpper))
                {
                    context.AddFailure("The password must contain at least one uppercase letter.");
                }

                if (!password.Any(char.IsLower))
                {
                    context.AddFailure("The password must contain at least one lowercase letter.");
                }

                if (!password.Any(char.IsDigit))
                {
                    context.AddFailure("The password must contain at least one number.");
                }

                if (!password.Any(IsSpecialCharacter))
                {
                    context.AddFailure("The password must contain at least one special character.");
                }
            });

        RuleFor(user => user.Role)
            .NotEmpty()
            .IsEnumName(typeof(SubmissionRoles))
            .WithMessage("A user must have a valid role.");

        RuleFor(user => user.Team)
            .NotNull()
            .When(user => string.Equals(user.Role, SubmissionRoles.Judge, StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("A participant must be associated with a team.");

        RuleFor(user => user.UserName)
            .NotEmpty()
            .WithMessage("A user must have a username.");
    }

    private static bool IsSpecialCharacter(char character)
    {
        return character switch
        {
            ' ' => true,
            '!' => true,
            '"' => true,
            '#' => true,
            '$' => true,
            '%' => true,
            '&' => true,
            '\'' => true,
            '(' => true,
            ')' => true,
            '*' => true,
            '+' => true,
            ',' => true,
            '-' => true,
            '.' => true,
            '/' => true,
            ':' => true,
            ';' => true,
            '<' => true,
            '=' => true,
            '>' => true,
            '?' => true,
            '@' => true,
            '[' => true,
            '\\' => true,
            ']' => true,
            '^' => true,
            '_' => true,
            '`' => true,
            '{' => true,
            '|' => true,
            '}' => true,
            '~' => true,
            _ => false
        };
    }
}
