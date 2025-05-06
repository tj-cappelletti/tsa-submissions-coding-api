using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
public class TeamsTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.HighSchool,
                Id = "000000000000000000000001",
                Participants =
                [
                    new Participant { SchoolNumber = "2000", ParticipantNumber = "001" },
                    new Participant { SchoolNumber = "2000", ParticipantNumber = "002" }
                ],
                SchoolNumber = "2000",
                TeamNumber = "901"
            },
            TeamDataIssues.None
        ];

        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.HighSchool,
                Id = "000000000000000000000002",
                Participants =
                [
                    new Participant { SchoolNumber = "2000", ParticipantNumber = "003" },
                    new Participant { SchoolNumber = "2000", ParticipantNumber = "004" }
                ],
                SchoolNumber = "2000",
                TeamNumber = "902"
            },
            TeamDataIssues.None
        ];


        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.HighSchool,
                Id = "000000000000000000000003",
                Participants =
                [
                    new Participant { SchoolNumber = "2000", ParticipantNumber = "005" },
                    new Participant { SchoolNumber = "2000", ParticipantNumber = "006" }
                ],
                SchoolNumber = "2000",
                TeamNumber = "903"
            },
            TeamDataIssues.None
        ];


        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.MiddleSchool,
                Id = "000000000000000000000004",
                Participants =
                [
                    new Participant { SchoolNumber = "1001", ParticipantNumber = "001" },
                    new Participant { SchoolNumber = "1001", ParticipantNumber = "002" }
                ],
                SchoolNumber = "1001",
                TeamNumber = "901"
            },
            TeamDataIssues.None
        ];


        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.MiddleSchool,
                Id = "000000000000000000000005",
                Participants =
                [
                    new Participant { SchoolNumber = "1001", ParticipantNumber = "002" },
                    new Participant { SchoolNumber = "1001", ParticipantNumber = "003" }
                ],
                SchoolNumber = "1001",
                TeamNumber = "902"
            },
            TeamDataIssues.None
        ];


        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.MiddleSchool,
                Id = "000000000000000000000006",
                Participants =
                [
                    new Participant { SchoolNumber = "1001", ParticipantNumber = "005" },
                    new Participant { SchoolNumber = "1001", ParticipantNumber = "006" }
                ],
                SchoolNumber = "1001",
                TeamNumber = "903"
            },
            TeamDataIssues.None
        ];

        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.MiddleSchool,
                Id = "000000000000000000000007",
                Participants =
                [
                    new Participant { SchoolNumber = null, ParticipantNumber = "005" },
                    new Participant { SchoolNumber = "2000", ParticipantNumber = "006" }
                ],
                SchoolNumber = "dog",
                TeamNumber = "901"
            },
            TeamDataIssues.InvalidParticipants | TeamDataIssues.InvalidSchoolNumber
        ];

        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.MiddleSchool,
                Id = "000000000000000000000007",
                Participants =
                [
                    new Participant { SchoolNumber = "2000", ParticipantNumber = null },
                    new Participant { SchoolNumber = "2000", ParticipantNumber = "006" }
                ],
                SchoolNumber = "dog",
                TeamNumber = "901"
            },
            TeamDataIssues.InvalidParticipants | TeamDataIssues.InvalidSchoolNumber
        ];

        yield return
        [
            new Team
            {
                Id = "000000000000000000000008",
                SchoolNumber = "dog",
                TeamNumber = "101"
            },
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidParticipants | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
        ];

        yield return
        [
            new Team
            {
                Id = "000000000000000000000009",
                Participants =
                [
                    new Participant
                    {
                        ParticipantNumber = "dog",
                        SchoolNumber = "9999"
                    }
                ],
                SchoolNumber = "dog",
                TeamNumber = "bird"
            },
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidParticipants | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
        ];

        yield return
        [
            new Team
            {
                Id = "00000000000000000000000a",
                SchoolNumber = "9999",
                TeamNumber = "901",
                Participants =
                [
                    new Participant
                    {
                        ParticipantNumber = "dog",
                        SchoolNumber = "dog"
                    },
                    new Participant
                    {
                        ParticipantNumber = "dog",
                        SchoolNumber = "dog"
                    },
                    new Participant
                    {
                        ParticipantNumber = "dog",
                        SchoolNumber = "dog"
                    }
                ]
            },
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidParticipants | TeamDataIssues.InvalidSchoolNumber
        ];

        yield return
        [
            new Team
            {
                Id = "00000000000000000000000b",
                Participants =
                [
                    new Participant { SchoolNumber = "9001", ParticipantNumber = "902" },
                    new Participant { SchoolNumber = "9001", ParticipantNumber = "003" }
                ],
                SchoolNumber = "9001",
                TeamNumber = "902"
            },
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidParticipants | TeamDataIssues.InvalidSchoolNumber
        ];
    }
}

[Flags]
public enum TeamDataIssues
{
    None = 0,
    InvalidCompetitionLevel = 1 << 0,
    InvalidParticipants = 1 << 1,
    InvalidSchoolNumber = 1 << 2,
    InvalidTeamNumber = 1 << 3
}
