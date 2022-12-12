using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.Tests.Data;

[ExcludeFromCodeCoverage]
public class ProblemsTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new Problem
            {
                Description = "This is the description of Problem #1",
                Id = "000000000000000000000001",
                IsActive = true,
                Title = "Problem #1"
            },
            ProblemDataIssues.None
        };

        yield return new object[]
        {
            new Problem
            {
                Description = "This is the description of Problem #2",
                Id = "000000000000000000000002",
                IsActive = true,
                Title = "Problem #2"
            },
            ProblemDataIssues.None
        };

        yield return new object[]
        {
            new Problem
            {
                Description = "This is the description of Problem #3",
                Id = "000000000000000000000003",
                IsActive = true,
                Title = "Problem #3"
            },
            ProblemDataIssues.None
        };

        yield return new object[]
        {
            new Problem
            {
                Description = "This is the description of Problem #4",
                Id = "000000000000000000000001",
                IsActive = false,
                Title = "Problem #4"
            },
            ProblemDataIssues.None
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[Flags]
public enum ProblemDataIssues
{
    None = 0
}