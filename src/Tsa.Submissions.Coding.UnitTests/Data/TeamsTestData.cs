using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
public class TeamsTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new Team
            {
                Id = "000000000000000000000001",
                Participants =
                [
                    new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                    new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                ],
                SchoolNumber = "9000",
                TeamNumber = "901"
            },
            TeamDataIssues.None
        };


        yield return new object[]
        {
            new Team
            {
                Id = "000000000000000000000002",
                Participants =
                [
                    new() { SchoolNumber = "9000", ParticipantNumber = "003" },
                    new() { SchoolNumber = "9000", ParticipantNumber = "004" }
                ],
                SchoolNumber = "9000",
                TeamNumber = "902"
            },
            TeamDataIssues.None
        };


        yield return new object[]
        {
            new Team
            {
                Id = "000000000000000000000003",
                Participants =
                [
                    new() { SchoolNumber = "9000", ParticipantNumber = "005" },
                    new() { SchoolNumber = "9000", ParticipantNumber = "006" }
                ],
                SchoolNumber = "9000",
                TeamNumber = "903"
            },
            TeamDataIssues.None
        };


        yield return new object[]
        {
            new Team
            {
                Id = "000000000000000000000004",
                Participants =
                [
                    new() { SchoolNumber = "9001", ParticipantNumber = "001" },
                    new() { SchoolNumber = "9001", ParticipantNumber = "002" }
                ],
                SchoolNumber = "9001",
                TeamNumber = "901"
            },
            TeamDataIssues.None
        };


        yield return new object[]
        {
            new Team
            {
                Id = "000000000000000000000005",
                Participants =
                [
                    new() { SchoolNumber = "9001", ParticipantNumber = "002" },
                    new() { SchoolNumber = "9001", ParticipantNumber = "003" }
                ],
                SchoolNumber = "9001",
                TeamNumber = "902"
            },
            TeamDataIssues.None
        };


        yield return new object[]
        {
            new Team
            {
                Id = "000000000000000000000006",
                Participants =
                [
                    new() { SchoolNumber = "9001", ParticipantNumber = "005" },
                    new() { SchoolNumber = "9001", ParticipantNumber = "006" }
                ],
                SchoolNumber = "9001",
                TeamNumber = "903"
            },
            TeamDataIssues.None
        };

        yield return new object[]
        {
            new Team
            {
                Id = "000000000000000000000007",
                Participants =
                [
                    new() { SchoolNumber = null, ParticipantNumber = "005" },
                    new() { SchoolNumber = "9000", ParticipantNumber = "006" }
                ],
                SchoolNumber = "dog",
                TeamNumber = "901"
            },
            TeamDataIssues.InvalidParticipants | TeamDataIssues.InvalidSchoolNumber
        };

        yield return new object[]
        {
            new Team
            {
                Id = "000000000000000000000007",
                Participants =
                [
                    new() { SchoolNumber = "9000", ParticipantNumber = null },
                    new() { SchoolNumber = "9000", ParticipantNumber = "006" }
                ],
                SchoolNumber = "dog",
                TeamNumber = "901"
            },
            TeamDataIssues.InvalidParticipants | TeamDataIssues.InvalidSchoolNumber
        };

        yield return new object[]
        {
            new Team
            {
                Id = "000000000000000000000008",
                SchoolNumber = "dog",
                TeamNumber = "101"
            },
            TeamDataIssues.InvalidParticipants | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
        };

        yield return new object[]
        {
            new Team
            {
                Id = "000000000000000000000009",
                Participants =
                [
                    new()
                    {
                        ParticipantNumber = "dog",
                        SchoolNumber = "9999"
                    }
                ],
                SchoolNumber = "dog",
                TeamNumber = "bird"
            },
            TeamDataIssues.InvalidParticipants | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
        };

        yield return new object[]
        {
            new Team
            {
                Id = "00000000000000000000000a",
                SchoolNumber = "9999",
                TeamNumber = "901",
                Participants =
                [
                    new()
                    {
                        ParticipantNumber = "dog",
                        SchoolNumber = "dog"
                    },
                    new()
                    {
                        ParticipantNumber = "dog",
                        SchoolNumber = "dog"
                    },
                    new()
                    {
                        ParticipantNumber = "dog",
                        SchoolNumber = "dog"
                    }
                ]
            },
            TeamDataIssues.InvalidParticipants
        };

        yield return new object[]
        {
            new Team
            {
                Id = "00000000000000000000000b",
                Participants =
                [
                    new() { SchoolNumber = "9001", ParticipantNumber = "902" },
                    new() { SchoolNumber = "9001", ParticipantNumber = "003" }
                ],
                SchoolNumber = "9001",
                TeamNumber = "902"
            },
            TeamDataIssues.InvalidParticipants
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[Flags]
public enum TeamDataIssues
{
    None = 0,
    InvalidParticipants = 1 << 0,
    InvalidSchoolNumber = 1 << 1,
    InvalidTeamNumber = 1 << 2
}
