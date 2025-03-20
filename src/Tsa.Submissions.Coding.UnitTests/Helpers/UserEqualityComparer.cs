using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
public class UserEqualityComparer : IEqualityComparer<User?>, IEqualityComparer<IList<User>?>
{
    public bool Equals(User? x, User? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var externalIdsMatch = x.ExternalId == y.ExternalId;
        var idsMatch = x.Id == y.Id;
        var rolesMatch = x.Role == y.Role;
        var teamsMatch = x.Team?.Id.AsString == y.Team?.Id.AsString;
        var userNamesMatch = x.UserName == y.UserName;

        return externalIdsMatch && idsMatch && rolesMatch && teamsMatch && userNamesMatch;
    }

    public bool Equals(IList<User>? x, IList<User>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftProblem in x)
        {
            var rightProblem = y.SingleOrDefault(user => user.Id == leftProblem.Id);

            if (!Equals(leftProblem, rightProblem)) return false;
        }

        return true;
    }

    public int GetHashCode(User? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<User>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
