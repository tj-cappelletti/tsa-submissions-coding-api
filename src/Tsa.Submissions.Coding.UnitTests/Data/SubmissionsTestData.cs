﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
internal class SubmissionsTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new Submission
            {
                Id = "000000000000000000000001",
                IsFinalSubmission = true,
                Language = "csharp",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000001"),
                Solution = "The solution",
                SubmittedOn = DateTime.Now,
                Team = new MongoDBRef(TeamsService.MongoDbCollectionName, "000000000000000000000001"),
                TestSetResults = new List<TestSetResult>
                {
                    new()
                    {
                        Passed = true,
                        RunDuration = new TimeSpan(0, 0, 5, 30),
                        TestSet = new MongoDBRef(TestSetsService.MongoDbCollectionName, "000000000000000000000001")
                    },
                    new()
                    {
                        Passed = false,
                        RunDuration = new TimeSpan(0, 0, 1, 30),
                        TestSet = new MongoDBRef(TestSetsService.MongoDbCollectionName, "000000000000000000000002")
                    }
                }
            },
            SubmissionDataIssues.None
        };
    }
}

[Flags]
public enum SubmissionDataIssues
{
    None = 0
}
