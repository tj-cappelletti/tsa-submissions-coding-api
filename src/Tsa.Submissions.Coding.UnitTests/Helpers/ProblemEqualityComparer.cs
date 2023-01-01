using System.Collections.Generic;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

internal class ProblemEqualityComparer : IEqualityComparer<Problem>
{
    public bool Equals(Problem? x, Problem? y)
    {
        return x?.Id == y?.Id;
    }

    public int GetHashCode(Problem obj)
    {
        return obj.GetHashCode();
    }
}
