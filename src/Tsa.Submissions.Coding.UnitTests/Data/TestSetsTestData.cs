using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
public class TestSetsTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000001",
                Inputs = new List<TestSetInput>
                {
                    new()
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new()
                    {
                        DataType = "number",
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                },
                IsPublic = true,
                Name = "Test Set #1",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000001")
            },
            TestSetDataIssues.None
        };

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000002",
                Inputs = new List<TestSetInput>
                {
                    new()
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new()
                    {
                        DataType = "number",
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                },
                IsPublic = true,
                Name = "Test Set #2",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000001")
            },
            TestSetDataIssues.None
        };

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000003",
                Inputs = new List<TestSetInput>
                {
                    new()
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new()
                    {
                        DataType = "number",
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                },
                IsPublic = true,
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000002")
            },
            TestSetDataIssues.MissingName
        };

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000004",
                Inputs = new List<TestSetInput>
                {
                    new()
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new()
                    {
                        DataType = "number",
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                },
                IsPublic = true,
                Name = "Test Set #4"
            },
            TestSetDataIssues.MissingProblemId
        };

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000005",
                IsPublic = true,
                Name = "Test Set #5",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000002")
            },
            TestSetDataIssues.MissingInput
        };
    }
}

[Flags]
public enum TestSetDataIssues
{
    None = 0,
    MissingName = 1 << 0,
    MissingProblemId = 1 << 1,
    MissingInput = 1 << 2
}
