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

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000006",
                Inputs = new List<TestSetInput>
                {
                    new()
                    {
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new()
                    {
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                },
                IsPublic = true,
                Name = "Test Set #6",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.MissingDataType
        };

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000007",
                Inputs = new List<TestSetInput>
                {
                    new()
                    {
                        DataType = "dog",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new()
                    {
                        DataType = "cat",
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                },
                IsPublic = true,
                Name = "Test Set #7",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.InvalidDataType
        };

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000008",
                Inputs = new List<TestSetInput>
                {
                    new()
                    {
                        DataType = "string",
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new()
                    {
                        DataType = "number",
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                },
                IsPublic = true,
                Name = "Test Set #8",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.MissingIndex
        };

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000009",
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
                        Index = 0,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                },
                IsPublic = true,
                Name = "Test Set #2",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.IndexNotUnique
        };

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000010",
                Inputs = new List<TestSetInput>
                {
                    new()
                    {
                        DataType = "string",
                        Index = 0
                    },
                    new()
                    {
                        DataType = "number",
                        Index = 1
                    }
                },
                IsPublic = true,
                Name = "Test Set #10",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.MissingValueAsJson
        };

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000011",
                Inputs = new List<TestSetInput>
                {
                    new()
                    {
                        DataType = "number",
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
                Name = "Test Set #11",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.ValueDoesNotMatchDataType
        };

        yield return new object[]
        {
            new TestSet
            {
                Id = "000000000000000000000012",
                Inputs = new List<TestSetInput>
                {
                    new()
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "Try me"
                    },
                    new()
                    {
                        DataType = "number",
                        Index = 1,
                        ValueAsJson = "Try me"
                    }
                },
                IsPublic = true,
                Name = "Test Set #12",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.ValueCannotBeDeserialized
        };
    }
}

[Flags]
public enum TestSetDataIssues
{
    None = 0,
    MissingName = 1 << 0,
    MissingProblemId = 1 << 1,
    MissingInput = 1 << 2,
    MissingDataType = 1 << 3,
    InvalidDataType = 1 << 4,
    MissingIndex = 1 << 5,
    IndexNotUnique = 1 << 6,
    MissingValueAsJson = 1 << 7,
    ValueDoesNotMatchDataType = 1 << 8,
    ValueCannotBeDeserialized = 1 << 9
}
