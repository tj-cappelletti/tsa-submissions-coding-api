using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static TestSetModel ToModel(this TestSet testSet)
    {
        return new TestSetModel
        {
            Id = testSet.Id,
            Inputs = testSet.Inputs.ToModels(),
            IsPublic = testSet.IsPublic,
            Name = testSet.Name,
            ProblemId = testSet.Problem?.Id.AsString
        };
    }

    public static List<TestSetModel> ToModels(this IList<TestSet> testSets)
    {
        return testSets.Select(testSet => testSet.ToModel()).ToList();
    }
}
