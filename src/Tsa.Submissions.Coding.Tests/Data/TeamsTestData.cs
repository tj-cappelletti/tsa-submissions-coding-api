using System;
using System.Collections;
using System.Collections.Generic;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.Tests.Data;

public class TeamsTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new Team
            {
                Id = "000000000001",
                Participants = new List<Participant>
                {
                    new() { SchoolNumber = "9000", ParticipantNumber = "001" },
                    new() { SchoolNumber = "9000", ParticipantNumber = "002" }
                },
                SchoolNumber = "9000",
                TeamNumber = "901"
            },
            TeamDataIssues.None
        };


        yield return new object[]
        {
            new Team
            {
                Id = "000000000002",
                Participants = new List<Participant>
                {
                    new() { SchoolNumber = "9000", ParticipantNumber = "003" },
                    new() { SchoolNumber = "9000", ParticipantNumber = "004" }
                },
                SchoolNumber = "9000",
                TeamNumber = "902"
            },
            TeamDataIssues.None
        };


        yield return new object[]
        {
            new Team
            {
                Id = "000000000003",
                Participants = new List<Participant>
                {
                    new() { SchoolNumber = "9000", ParticipantNumber = "005" },
                    new() { SchoolNumber = "9000", ParticipantNumber = "006" }
                },
                SchoolNumber = "9000",
                TeamNumber = "903"
            },
            TeamDataIssues.None
        };


        yield return new object[]
        {
            new Team
            {
                Id = "000000000004",
                Participants = new List<Participant>
                {
                    new() { SchoolNumber = "9001", ParticipantNumber = "001" },
                    new() { SchoolNumber = "9001", ParticipantNumber = "002" }
                },
                SchoolNumber = "9001",
                TeamNumber = "901"
            },
            TeamDataIssues.None
        };


        yield return new object[]
        {
            new Team
            {
                Id = "000000000005",
                Participants = new List<Participant>
                {
                    new() { SchoolNumber = "9001", ParticipantNumber = "002" },
                    new() { SchoolNumber = "9001", ParticipantNumber = "003" }
                },
                SchoolNumber = "9001",
                TeamNumber = "902"
            },
            TeamDataIssues.None
        };


        yield return new object[]
        {
            new Team
            {
                Id = "000000000006",
                Participants = new List<Participant>
                {
                    new() { SchoolNumber = "9000", ParticipantNumber = "005" },
                    new() { SchoolNumber = "9000", ParticipantNumber = "006" }
                },
                SchoolNumber = "9001",
                TeamNumber = "903"
            },
            TeamDataIssues.None
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
    None = 0
}
