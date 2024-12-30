using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    public static TestSetValueModel ToModel(this TestSetValue testSetValue)
    {
        return new TestSetValueModel
        {
            DataType = testSetValue.DataType,
            Index = testSetValue.Index,
            IsArray = testSetValue.IsArray,
            ValueAsJson = testSetValue.ValueAsJson
        };
    }

    public static List<TestSetValueModel>? ToModels(this IList<TestSetValue>? testSetInputs)
    {
        return testSetInputs?.Select(testSetInput => testSetInput.ToModel()).ToList();
    }
}
