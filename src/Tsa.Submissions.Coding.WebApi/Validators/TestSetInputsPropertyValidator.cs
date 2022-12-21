using System;
using System.Collections.Generic;
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

        foreach (var testSetInputModel in value)
        {
            if (string.IsNullOrWhiteSpace(testSetInputModel.DataType) || !IsValidDataType(testSetInputModel.DataType))
            {
                context.AddFailure(nameof(TestSetInputModel.DataType), "You must specify a valid data type for the input");
                return false;
            }

            if (testSetInputModel.Index == null)
            {
                context.AddFailure(nameof(TestSetInputModel.Index), "There must be a value for the index");
                return false;
            }

            if (string.IsNullOrWhiteSpace(testSetInputModel.ValueAsJson))
            {
                context.AddFailure(nameof(TestSetInputModel.ValueAsJson), "You must specify a value for the input");
                return false;
            }

            if (!ValueParsesToDataType(testSetInputModel))
            {
                context.AddFailure(nameof(TestSetInputModel.ValueAsJson), "The value must parse to the specified data type.");
                return false;
            }
        }

        return true;
    }

    private static bool IsValidDataType(string dataType)
    {
        return Enum.TryParse<TestSetInputDataTypes>(dataType, out _);
    }

    private static bool IsValidJsonType<T>(string jsonValue)
    {
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
        if (!Enum.TryParse<TestSetInputDataTypes>(testSetInputModel.DataType, out var testSetInputDataType))
        {
            throw new ArgumentException("Unable to determine the data type of the value.");
        }

        if (string.IsNullOrWhiteSpace(testSetInputModel.ValueAsJson))
        {
            throw new ArgumentException("The value cannot be null.", nameof(TestSetInputModel.ValueAsJson));
        }

        return testSetInputDataType switch
        {
            TestSetInputDataTypes.Character => testSetInputModel.IsArray
                ? IsValidJsonType<ValueAsCharacterArrayModel>(testSetInputModel.ValueAsJson)
                : IsValidJsonType<ValueAsCharacterModel>(testSetInputModel.ValueAsJson),

            TestSetInputDataTypes.Decimal => testSetInputModel.IsArray
                ? IsValidJsonType<ValueAsDecimalArrayModel>(testSetInputModel.ValueAsJson)
                : IsValidJsonType<ValueAsDecimalModel>(testSetInputModel.ValueAsJson),

            TestSetInputDataTypes.Number => testSetInputModel.IsArray
                ? IsValidJsonType<ValueAsNumberArrayModel>(testSetInputModel.ValueAsJson)
                : IsValidJsonType<ValueAsNumberModel>(testSetInputModel.ValueAsJson),

            TestSetInputDataTypes.String => testSetInputModel.IsArray
                ? IsValidJsonType<ValueAsStringArrayModel>(testSetInputModel.ValueAsJson)
                : IsValidJsonType<ValueAsStringModel>(testSetInputModel.ValueAsJson),

            _ => throw new NotImplementedException($"Parsing for the `{testSetInputDataType}` is not supported.")
        };
    }
}
