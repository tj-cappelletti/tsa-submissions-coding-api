using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.Tests.Data;

[ExcludeFromCodeCoverage]
internal class InvalidTeamTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Bad School Number
        yield return new object[] { new Team { SchoolNumber = "dog", TeamNumber = "901" } };

        // Bad School Number and Team Number
        yield return new object[] { new Team { SchoolNumber = "dog", TeamNumber = "101" } };
        yield return new object[] { new Team { SchoolNumber = "dog", TeamNumber = "bird" } };

        // Bad Team Number
        yield return new object[] { new Team { SchoolNumber = "9999", TeamNumber = "101" } };
        yield return new object[] { new Team { SchoolNumber = "9999", TeamNumber = "bird" } };

        // Bad School Number and Team Number with Bad Participants
        yield return new object[]
        {
            new Team
            {
                SchoolNumber = "dog",
                TeamNumber = "bird",
                Participants = new List<Participant>
                {
                    new() {ParticipantNumber = "dog", SchoolNumber = "dog"}
                }
            }
        };

        // Good School Number and Team Number with Bad Participants
        yield return new object[]
        {
            new Team
            {
                SchoolNumber = "9999",
                TeamNumber = "901",
                Participants = new List<Participant>
                {
                    new() {ParticipantNumber = "dog", SchoolNumber = "dog"}
                }
            }
        };

        yield return new object[]
        {
            new Team
            {
                SchoolNumber = "9999",
                TeamNumber = "901",
                Participants = new List<Participant>
                {
                    new() {ParticipantNumber = "101", SchoolNumber = "9999"},
                    new() {ParticipantNumber = "102", SchoolNumber = "9999"},
                    new() {ParticipantNumber = "103", SchoolNumber = "9999"}
                }
            }
        };

        yield return new object[]
        {
            new Team
            {
                SchoolNumber = "9999",
                TeamNumber = "901",
                Participants = new List<Participant>
                {
                    new() {ParticipantNumber = "901", SchoolNumber = "9999"},
                    new() {ParticipantNumber = "102", SchoolNumber = "9999"}
                }
            }
        };

        yield return new object[]
        {
            new Team
            {
                SchoolNumber = "9999",
                TeamNumber = "901",
                Participants = new List<Participant>
                {
                    new() {ParticipantNumber = "102", SchoolNumber = "9999"},
                    new() {ParticipantNumber = "102", SchoolNumber = "9999"}
                }
            }
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
