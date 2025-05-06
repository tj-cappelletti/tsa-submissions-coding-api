using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class TeamEqualityComparer : IEqualityComparer<Team?>, IEqualityComparer<IList<Team>?>
{
    private readonly ParticipantEqualityComparer _participantModelEqualityComparer = new();

    public bool Equals(Team? x, Team? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var competitionLevelsMatch = x.CompetitionLevel == y.CompetitionLevel;
        var idsMatch = x.Id == y.Id;
        var participantsMatch = _participantModelEqualityComparer.Equals(x.Participants, y.Participants);
        var schoolNumbersMatch = x.SchoolNumber == y.SchoolNumber;
        var teamNumbersMatch = x.TeamNumber == y.TeamNumber;

        return competitionLevelsMatch &&
               idsMatch &&
               participantsMatch &&
               schoolNumbersMatch &&
               teamNumbersMatch;
    }

    public bool Equals(IList<Team>? x, IList<Team>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftTeamModel in x)
        {
            var rightTeamModel = y.SingleOrDefault(_ => _.Id == leftTeamModel.Id);

            if (!Equals(leftTeamModel, rightTeamModel)) return false;
        }

        return true;
    }

    public int GetHashCode(Team? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<Team>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
