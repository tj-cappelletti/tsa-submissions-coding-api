using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class TestSetInputModelEqualityComparer : IEqualityComparer<TestSetInputModel?>, IEqualityComparer<IList<TestSetInputModel>?>
{
    public bool Equals(TestSetInputModel? x, TestSetInputModel? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        return
            x.DataType == y.DataType &&
            x.Index == y.Index &&
            x.IsArray == y.IsArray &&
            x.ValueAsJson == y.ValueAsJson;
    }

    public bool Equals(IList<TestSetInputModel>? x, IList<TestSetInputModel>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftTestInputModel in x)
        {
            var rightTestSetInputModel = y.SingleOrDefault(_ => _.Index == leftTestInputModel.Index);

            if (!Equals(leftTestInputModel, rightTestSetInputModel)) return false;
        }

        return true;
    }

    public int GetHashCode(TestSetInputModel obj)
    {
        return HashCode.Combine(obj.DataType, obj.Index, obj.IsArray, obj.ValueAsJson);
    }

    public int GetHashCode(IList<TestSetInputModel>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
