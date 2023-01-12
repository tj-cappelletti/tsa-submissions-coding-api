﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using Newtonsoft.Json;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class TestSetInputsPropertyValidator : PropertyValidator<TestSetModel, IList<TestSetInputModel>?>
{
    public override string Name => "TestSetInputsPropertyValidator";

    public override bool IsValid(ValidationContext<TestSetModel> context, IList<TestSetInputModel>? value)
    {
        if (value == null || value.Count == 0)
        {
            context.AddFailure("At least one input for the test is required");
            return false;
        }

        var indexes = new List<int>();

        foreach (var testSetInputModel in value)
        {
            if (string.IsNullOrWhiteSpace(testSetInputModel.DataType) || !IsValidDataType(testSetInputModel.DataType))
            {
                context.AddFailure("You must specify a valid data type for the input");
                return false;
            }

            if (testSetInputModel.Index == null)
            {
                context.AddFailure("There must be a value for the index");
                return false;
            }

            if (indexes.Contains(testSetInputModel.Index.Value))
            {
                context.AddFailure("The value for the input index must be unique");
                return false;
            }

            indexes.Add(testSetInputModel.Index.Value);

            if (string.IsNullOrWhiteSpace(testSetInputModel.ValueAsJson))
            {
                context.AddFailure("You must specify a value for the input");
                return false;
            }

            if (!ValueParsesToDataType(testSetInputModel))
            {
                context.AddFailure($"Unable to deserialize the value `{testSetInputModel.ValueAsJson}` to the specified data type `{testSetInputModel.DataType}`");
                return false;
            }
        }

        var expectedValue = 0;
        foreach (var index in indexes.Order())
        {
            if (index != expectedValue)
            {
                context.AddFailure("Indexes need to start at 0 and must be continuous.");
                return false;
            }

            expectedValue++;
        }

        return true;
    }

    private static bool IsValidDataType(string dataType)
    {
        return Enum.TryParse<TestSetInputDataTypes>(dataType, true, out _);
    }

    private static bool IsValidJsonType<T>(string? jsonValue)
    {
        if (jsonValue == null) return false;

        try
        {
            JsonConvert.DeserializeObject<T>(jsonValue);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    private static bool ValueParsesToDataType(TestSetInputModel testSetInputModel)
    {
        // This method should only be called after testSetInputModel.DataType
        // has been verified to have to be a valid value
        var testSetInputDataType = Enum.Parse<TestSetInputDataTypes>(testSetInputModel.DataType!, true);

        return testSetInputDataType switch
        {
            TestSetInputDataTypes.Character => testSetInputModel.IsArray
                ? IsValidJsonType<ValueAsCharacterArrayModel>(testSetInputModel.ValueAsJson!)
                : IsValidJsonType<ValueAsCharacterModel>(testSetInputModel.ValueAsJson!),

            TestSetInputDataTypes.Decimal => testSetInputModel.IsArray
                ? IsValidJsonType<ValueAsDecimalArrayModel>(testSetInputModel.ValueAsJson)
                : IsValidJsonType<ValueAsDecimalModel>(testSetInputModel.ValueAsJson),

            TestSetInputDataTypes.Number => testSetInputModel.IsArray
                ? IsValidJsonType<ValueAsNumberArrayModel>(testSetInputModel.ValueAsJson)
                : IsValidJsonType<ValueAsNumberModel>(testSetInputModel.ValueAsJson),

            TestSetInputDataTypes.String => testSetInputModel.IsArray
                ? IsValidJsonType<ValueAsStringArrayModel>(testSetInputModel.ValueAsJson)
                : IsValidJsonType<ValueAsStringModel>(testSetInputModel.ValueAsJson),

            _ => throw new NotImplementedException($"Parsing for the `{testSetInputDataType}` data type is not supported")
        };
    }
}
