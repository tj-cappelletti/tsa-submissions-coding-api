﻿using System.Diagnostics.CodeAnalysis;
using FluentValidation.TestHelper;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Validators;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Validators;

[ExcludeFromCodeCoverage]
public class TestSetModelValidatorTests
{
    [Theory]
    [ClassData(typeof(TestSetsTestData))]
    [Trait("TestCategory", "UnitTest")]
    public void Should_Successfully_Validate_TeamModel(TestSet testSet, TestSetDataIssues testSetDataIssues)
    {
        // Arrange
        var teamModel = testSet.ToModel();

        var testSetModelValidator = new TestSetModelValidator();

        // Act

        var testValidationResult = testSetModelValidator.TestValidate(teamModel);

        // Assert

        if (testSetDataIssues.HasFlag(TestSetDataIssues.MissingName))
        {
            testValidationResult.ShouldHaveValidationErrorFor(_ => _.Name);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        if (testSetDataIssues.HasFlag(TestSetDataIssues.MissingProblemId))
        {
            testValidationResult.ShouldHaveValidationErrorFor(_ => _.ProblemId);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(_ => _.ProblemId);
        }

        if (testSetDataIssues.HasFlag(TestSetDataIssues.MissingInput) ||
            testSetDataIssues.HasFlag(TestSetDataIssues.MissingDataType) ||
            testSetDataIssues.HasFlag(TestSetDataIssues.InvalidDataType) ||
            testSetDataIssues.HasFlag(TestSetDataIssues.MissingIndex) ||
            testSetDataIssues.HasFlag(TestSetDataIssues.IndexNotUnique) ||
            testSetDataIssues.HasFlag(TestSetDataIssues.MissingValueAsJson) ||
            testSetDataIssues.HasFlag(TestSetDataIssues.ValueDoesNotMatchDataType) ||
            testSetDataIssues.HasFlag(TestSetDataIssues.ValueCannotBeDeserialized) ||
            testSetDataIssues.HasFlag(TestSetDataIssues.IndexNotContinuous))
        {
            testValidationResult.ShouldHaveValidationErrorFor(_ => _.Inputs);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(_ => _.Inputs);
        }
    }
}
