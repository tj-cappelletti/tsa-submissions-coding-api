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
            UserModelsDataIssues.None
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelsDataIssues.None
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "003" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "004" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "902"
                },
                UserName = "9000-902"
            },
            UserModelsDataIssues.None
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "005" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "006" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "903"
                },
                UserName = "9000-903"
            },
            UserModelsDataIssues.None
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9001", ParticipantNumber = "001" },
                        new() { SchoolNumber = "9001", ParticipantNumber = "002" }
                    },
                    SchoolNumber = "9001",
                    TeamNumber = "901"
                },
                UserName = "9001-901"
            },
            UserModelsDataIssues.None
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9001", ParticipantNumber = "003" },
                        new() { SchoolNumber = "9001", ParticipantNumber = "004" }
                    },
                    SchoolNumber = "9001",
                    TeamNumber = "902"
                },
                UserName = "9001-902"
            },
            UserModelsDataIssues.None
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9001", ParticipantNumber = "005" },
                        new() { SchoolNumber = "9001", ParticipantNumber = "006" }
                    },
                    SchoolNumber = "9001",
                    TeamNumber = "903"
                },
                UserName = "9001-903"
            },
            UserModelsDataIssues.None
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelsDataIssues.PasswordLength
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelsDataIssues.PasswordMissingUppercase
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelsDataIssues.PasswordMissingLowercase
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelsDataIssues.PasswordMissingNumber
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelsDataIssues.PasswordMissingSpecialCharacter
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Team = new TeamModel
                {
                    Id = "000000000000000000000001",
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelsDataIssues.RoleNotValid
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelsDataIssues.RoleNotValid
        ];

        yield return
        [
            new UserModel
            {
                Password = "Password1!",
                Role = "participant",
                UserName = "9000-901"
            },
            UserModelsDataIssues.ParticipantMissingTeam
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
                    Participants = new List<ParticipantModel>
                    {
                        new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                        new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                    },
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = ""
            },
            UserModelsDataIssues.UserNameNotValid
        ];
    }
}

[Flags]
public enum UserModelsDataIssues
{
    None = 0,
    PasswordLength = 1 << 0,
    PasswordMissingUppercase = 1 << 1,
    PasswordMissingLowercase = 1 << 2,
    PasswordMissingNumber = 1 << 3,
    PasswordMissingSpecialCharacter = 1 << 4,
    RoleNotValid = 1 << 5,
    ParticipantMissingTeam = 1 << 6,
    UserNameNotValid = 1 << 7
}
