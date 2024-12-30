﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class TeamModelEqualityComparer : IEqualityComparer<TeamModel?>, IEqualityComparer<IList<TeamModel>?>
{
    private readonly ParticipantModelEqualityComparer _participantModelEqualityComparer = new();

    public bool Equals(TeamModel? x, TeamModel? y)
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

    public bool Equals(IList<TeamModel>? x, IList<TeamModel>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftTeamModel in x)
        {
            var rightTeamModel = y.SingleOrDefault(teamModel => teamModel.Id == leftTeamModel.Id);

            if (!Equals(leftTeamModel, rightTeamModel)) return false;
        }

        return true;
    }

    public int GetHashCode(TeamModel? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<TeamModel>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
