using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
public class UserModelsTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = null,
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb ",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb!",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb\"",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb$",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb%",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb&",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb'",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb(",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb)",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb*",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb,",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb-",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb.",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb/",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb:",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb;",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb<",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb=",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb>",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb?",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb@",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb[",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb\\",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb]",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb^",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb_",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb`",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb{",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb|",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb~",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Aaaa1Bbbb}",
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000001",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "002" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000002",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "003" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "004" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "902"
                },
                UserName = "9000-902"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000003",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "005" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "006" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "903"
                },
                UserName = "9000-903"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000004",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9001", ParticipantNumber = "001" },
                        new ParticipantModel { SchoolNumber = "9001", ParticipantNumber = "002" }
                    ],
                    SchoolNumber = "9001",
                    TeamNumber = "901"
                },
                UserName = "9001-901"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000005",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9001", ParticipantNumber = "003" },
                        new ParticipantModel { SchoolNumber = "9001", ParticipantNumber = "004" }
                    ],
                    SchoolNumber = "9001",
                    TeamNumber = "902"
                },
                UserName = "9001-902"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000006",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9001", ParticipantNumber = "005" },
                        new ParticipantModel { SchoolNumber = "9001", ParticipantNumber = "006" }
                    ],
                    SchoolNumber = "9001",
                    TeamNumber = "903"
                },
                UserName = "9001-903"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Password = "Pa1!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000001",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "002" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.PasswordLength
        ];

        yield return
        [
            new UserModel
            {
                Password = "password1!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000001",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "002" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.PasswordMissingUppercase
        ];

        yield return
        [
            new UserModel
            {
                Password = "PASSWORD1!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000001",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "002" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.PasswordMissingLowercase
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000001",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "002" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.PasswordMissingNumber
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000001",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "002" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.PasswordMissingSpecialCharacter
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Team = new TeamModel
                {
                    Id = "000000000000000000000001",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "002" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.RoleNotValid
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "dog",
                Team = new TeamModel
                {
                    Id = "000000000000000000000001",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "002" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.RoleNotValid
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "participant",
                UserName = "9000-901"
            },
            UserModelDataIssues.ParticipantMissingTeam
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "participant",
                Team = new TeamModel
                {
                    Id = "000000000000000000000001",
                    Participants =
                    [
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new ParticipantModel { SchoolNumber = "9000", ParticipantNumber = "002" }
                    ],
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = ""
            },
            UserModelDataIssues.UserNameNotValid
        ];
    }
}

[Flags]
public enum UserModelDataIssues
{
    None = 0,
    PasswordLength = 1 << 0,
    PasswordMissingLowercase = 1 << 1,
    PasswordMissingNumber = 1 << 2,
    PasswordMissingSpecialCharacter = 1 << 3,
    PasswordMissingUppercase = 1 << 4,
    RoleNotValid = 1 << 5,
    ParticipantMissingTeam = 1 << 6,
    UserNameNotValid = 1 << 7
}
