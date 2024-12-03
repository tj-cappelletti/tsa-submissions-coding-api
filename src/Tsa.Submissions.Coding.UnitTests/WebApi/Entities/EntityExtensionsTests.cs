using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Entities;

[ExcludeFromCodeCoverage]
public class EntityExtensions
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Submission_Should_Return_SubmissionModel(bool includeTestTests)
    {
        // Arrange
        var submission = new Submission
        {
            Id = "000000000000000000000000",
            IsFinalSubmission = true,
            Language = "csharp",
            Problem = new MongoDBRef("problems", "00000000000000000000000A"),
            Solution = "The solution",
            SubmittedOn = DateTime.Now.AddHours(-5),
            Team = new MongoDBRef("teams", "00000000000000000000000B")
        };

        if (includeTestTests)
        {
            submission.TestSetResults =
            [
                new()
                {
                    Passed = true,
                    RunDuration = new TimeSpan(0, 0, 5, 0),
                    TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
                },
                new()
                {
                    Passed = true,
                    RunDuration = new TimeSpan(0, 0, 5, 0),
                    TestSet = new MongoDBRef("test-sets", "000000000000000000000011")
                }
            ];
        }

        // Act
        var submissionModel = submission.ToModel();

        Assert.Equal(submission.Id, submissionModel.Id);
        Assert.Equal(submission.IsFinalSubmission, submissionModel.IsFinalSubmission);
        Assert.Equal(submission.Language, submissionModel.Language);
        Assert.Equal(submission.Problem?.Id.AsString, submissionModel.ProblemId);
        Assert.Equal(submission.Solution, submissionModel.Solution);
        Assert.Equal(submission.SubmittedOn, submissionModel.SubmittedOn);
        Assert.Equal(submission.Team?.Id.AsString, submissionModel.TeamId);

        if (includeTestTests)
        {
            Assert.NotNull(submissionModel.TestSetResults);
            Assert.Equal(submission.TestSetResults!.Count, submissionModel.TestSetResults.Count);
            foreach (var testSetResult in submission.TestSetResults)
            {
                var testSetResultModel = submissionModel.TestSetResults.SingleOrDefault(_ => _.TestSetId == testSetResult.TestSet?.Id.AsString);

                Assert.NotNull(testSetResultModel);
                Assert.Equal(testSetResult.Passed, testSetResultModel.Passed);
                Assert.Equal(testSetResult.RunDuration, testSetResultModel.RunDuration);
                Assert.Equal(testSetResult.TestSet?.Id.AsString, testSetResultModel.TestSetId);
            }
        }
        else
        {
            Assert.Null(submissionModel.TestSetResults);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Participant_Should_Return_ParticipantModel()
    {
        // Arrange
        var participant = new Participant
        {
            ParticipantNumber = "123",
            SchoolNumber = "1234"
        };

        // Act
        var participantModel = participant.ToModel();

        // Assert
        Assert.Equal(participant.ParticipantNumber, participantModel.ParticipantNumber);
        Assert.Equal(participant.SchoolNumber, participantModel.SchoolNumber);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Problem_Should_Return_ProblemModel()
    {
        // Arrange
        var problem = new Problem
        {
            Description = "This is the description",
            Id = "This is the ID",
            IsActive = true,
            Title = "This is the title"
        };

        // Act
        var problemModel = problem.ToModel();

        // Assert
        Assert.Equal(problem.Description, problemModel.Description);
        Assert.Equal(problem.Id, problemModel.Id);
        Assert.Equal(problem.IsActive, problemModel.IsActive);
        Assert.Equal(problem.Title, problemModel.Title);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Team_Should_Return_TeamModel()
    {
        // Arrange
        var team = new Team
        {
            Id = "This is an ID",
            Participants =
            [
                new()
                {
                    ParticipantNumber = "001",
                    SchoolNumber = "1234"
                },
                new()
                {
                    ParticipantNumber = "002",
                    SchoolNumber = "1234"
                }
            ],
            SchoolNumber = "1234",
            TeamNumber = "901"
        };

        // Act
        var teamModel = team.ToModel();

        // Assert
        Assert.Equal(team.Id, teamModel.Id);
        Assert.Equal(team.SchoolNumber, teamModel.SchoolNumber);
        Assert.Equal(team.TeamNumber, teamModel.TeamNumber);

        Assert.Equal(team.Participants.Count, teamModel.Participants.Count);

        foreach (var teamModelParticipant in teamModel.Participants)
        {
            Assert.Contains(team.Participants, _ => _.ParticipantId == teamModelParticipant.ParticipantId);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_TestSet_Should_Return_TestSetModel()
    {
        // Arrange
        var testSet = new TestSet
        {
            Id = "This is an ID",
            Inputs =
            [
                new()
                {
                    DataType = "Data Type #1",
                    Index = 1,
                    IsArray = true,
                    ValueAsJson = "ValueAsJson #1"
                },
                new()
                {
                    DataType = "Data Type #2",
                    Index = 2,
                    IsArray = false,
                    ValueAsJson = "ValueAsJson #2"
                },
                new()
                {
                    DataType = "Data Type #3",
                    Index = 3,
                    IsArray = true,
                    ValueAsJson = "ValueAsJson #3"
                }
            ],
            IsPublic = true,
            Name = "Test Set #1",
            Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000000")
        };

        // Act
        var testSetModel = testSet.ToModel();

        // Assert
        Assert.Equal(testSet.Id, testSetModel.Id);
        Assert.NotNull(testSetModel.Inputs);
        Assert.NotEmpty(testSetModel.Inputs);

        foreach (var testSetInput in testSet.Inputs)
        {
            var testSetInputModel = testSetModel.Inputs.SingleOrDefault(_ => _.Index == testSetInput.Index);

            Assert.NotNull(testSetInputModel);
            Assert.Equal(testSetInput.DataType, testSetInputModel.DataType);
            Assert.Equal(testSetInput.IsArray, testSetInputModel.IsArray);
            Assert.Equal(testSetInput.ValueAsJson, testSetInputModel.ValueAsJson);
        }

        Assert.Equal(testSet.Name, testSetModel.Name);
        Assert.Equal(testSet.Problem?.Id.AsString, testSetModel.ProblemId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_TestSet_Should_Return_TestSetModel_With_ProblemId_Null_When_Problem_Is_Null()
    {
        // Arrange
        var testSet = new TestSet
        {
            Id = "This is an ID",
            Inputs =
            [
                new()
                {
                    DataType = "Data Type #1",
                    Index = 1,
                    ValueAsJson = "ValueAsJson #1"
                },
                new()
                {
                    DataType = "Data Type #2",
                    Index = 2,
                    ValueAsJson = "ValueAsJson #2"
                },
                new()
                {
                    DataType = "Data Type #3",
                    Index = 3,
                    ValueAsJson = "ValueAsJson #3"
                }
            ],
            IsPublic = true,
            Name = "Test Set #1",
            Problem = null
        };

        // Act
        var testSetModel = testSet.ToModel();

        // Assert
        Assert.Equal(testSet.Id, testSetModel.Id);
        Assert.NotNull(testSetModel.Inputs);
        Assert.NotEmpty(testSetModel.Inputs);

        foreach (var testSetInput in testSet.Inputs)
        {
            var testSetInputModel = testSetModel.Inputs.SingleOrDefault(_ => _.Index == testSetInput.Index);

            Assert.NotNull(testSetInputModel);
            Assert.Equal(testSetInput.DataType, testSetInputModel.DataType);
            Assert.Equal(testSetInput.ValueAsJson, testSetInputModel.ValueAsJson);
        }

        Assert.Equal(testSet.Name, testSetModel.Name);
        Assert.Null(testSetModel.ProblemId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_TestSetInput_Should_Return_TestSetInputModel()
    {
        // Arrange
        var testSetInput = new TestSetValue
        {
            DataType = "Data Type",
            Index = 9999,
            IsArray = true,
            ValueAsJson = "ValueAsJson"
        };

        // Act
        var testSetInputModel = testSetInput.ToModel();

        // Assert
        Assert.Equal(testSetInput.DataType, testSetInputModel.DataType);
        Assert.Equal(testSetInput.Index, testSetInputModel.Index);
        Assert.Equal(testSetInput.IsArray, testSetInputModel.IsArray);
        Assert.Equal(testSetInput.ValueAsJson, testSetInputModel.ValueAsJson);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_TestSetResult_Should_Return_TestSetResultModel()
    {
        // Arrange
        var testSetResult = new TestSetResult
        {
            Passed = true,
            RunDuration = new TimeSpan(0, 0, 5, 0),
            TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
        };

        // Act
        var testSetResultModel = testSetResult.ToModel();

        // Assert
        Assert.NotNull(testSetResultModel);
        Assert.Equal(testSetResult.Passed, testSetResultModel.Passed);
        Assert.Equal(testSetResult.RunDuration, testSetResultModel.RunDuration);
        Assert.Equal(testSetResult.TestSet?.Id.AsString, testSetResultModel.TestSetId);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModels_For_Submission_Should_Return_SubmissionModel()
    {
        // Arrange
        var submissions = new List<Submission>
        {
            new()
            {
                Id = "000000000000000000000000",
                IsFinalSubmission = true,
                Language = "csharp",
                Problem = new MongoDBRef("problems", "00000000000000000000000A"),
                Solution = "The solution in C#",
                SubmittedOn = DateTime.Now.AddHours(-5),
                Team = new MongoDBRef("teams", "00000000000000000000000B"),
                TestSetResults =
                [
                    new()
                    {
                        Passed = true,
                        RunDuration = new TimeSpan(0, 0, 5, 0),
                        TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
                    },
                    new()
                    {
                        Passed = true,
                        RunDuration = new TimeSpan(0, 0, 5, 0),
                        TestSet = new MongoDBRef("test-sets", "000000000000000000000011")
                    }
                ]
            },
            new()
            {
                Id = "000000000000000000000001",
                IsFinalSubmission = true,
                Language = "java",
                Problem = new MongoDBRef("problems", "00000000000000000000000A"),
                Solution = "The solution in Java",
                SubmittedOn = DateTime.Now.AddHours(-4),
                Team = new MongoDBRef("teams", "00000000000000000000000C"),
                TestSetResults =
                [
                    new()
                    {
                        Passed = true,
                        RunDuration = new TimeSpan(0, 0, 5, 0),
                        TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
                    },
                    new()
                    {
                        Passed = true,
                        RunDuration = new TimeSpan(0, 0, 5, 0),
                        TestSet = new MongoDBRef("test-sets", "000000000000000000000011")
                    }
                ]
            }
        };

        // Act


        var submissionModels = submissions.ToModels();

        foreach (var submission in submissions)
        {
            var submissionModel = submissionModels.SingleOrDefault(_ => _.Id == submission.Id);

            Assert.NotNull(submissionModel);
            Assert.Equal(submission.Id, submissionModel.Id);
            Assert.Equal(submission.IsFinalSubmission, submissionModel.IsFinalSubmission);
            Assert.Equal(submission.Language, submissionModel.Language);
            Assert.Equal(submission.Problem?.Id.AsString, submissionModel.ProblemId);
            Assert.Equal(submission.Solution, submissionModel.Solution);
            Assert.Equal(submission.SubmittedOn, submissionModel.SubmittedOn);
            Assert.Equal(submission.Team?.Id.AsString, submissionModel.TeamId);
            Assert.NotNull(submissionModel.TestSetResults);
            Assert.Equal(submission.TestSetResults!.Count, submissionModel.TestSetResults.Count);

            foreach (var testSetResult in submission.TestSetResults)
            {
                var testSetResultModel = submissionModel.TestSetResults.SingleOrDefault(_ => _.TestSetId == testSetResult.TestSet?.Id.AsString);

                Assert.NotNull(testSetResultModel);
                Assert.Equal(testSetResult.Passed, testSetResultModel.Passed);
                Assert.Equal(testSetResult.RunDuration, testSetResultModel.RunDuration);
                Assert.Equal(testSetResult.TestSet?.Id.AsString, testSetResultModel.TestSetId);
            }
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModels_For_TestSetInputs_Should_Return_Null_When_Inputs_Is_Null()
    {
        // Arrange
        var testSet = new TestSet();

        // Act
        var testSetInputModels = testSet.Inputs.ToModels();

        // Assert
        Assert.Null(testSetInputModels);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModels_For_TestSetResults_Should_Return_TestSetResultModels()
    {
        // Arrange
        var testSetResults = new List<TestSetResult>
        {
            new()
            {
                Passed = true,
                RunDuration = new TimeSpan(0, 0, 5, 0),
                TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
            },
            new()
            {
                Passed = true,
                RunDuration = new TimeSpan(0, 0, 5, 0),
                TestSet = new MongoDBRef("test-sets", "000000000000000000000011")
            }
        };

        // Act
        var testSetResultModels = testSetResults.ToModels();

        // Assert
        Assert.NotNull(testSetResultModels);

        foreach (var testSetResult in testSetResults)
        {
            var testSetResultModel = testSetResultModels.SingleOrDefault(_ => _.TestSetId == testSetResult.TestSet?.Id.AsString);

            Assert.NotNull(testSetResultModel);
            Assert.Equal(testSetResult.Passed, testSetResultModel.Passed);
            Assert.Equal(testSetResult.RunDuration, testSetResultModel.RunDuration);
            Assert.Equal(testSetResult.TestSet?.Id.AsString, testSetResultModel.TestSetId);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModels_For_TestSetResults_When_Null_Should_Return_Null()
    {
        // Arrange
        List<TestSetResult>? testSetResults = null;

        // Act
        var testSetResultModels = testSetResults.ToModels();

        // Assert
        Assert.Null(testSetResultModels);
    }


    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModels_For_TestSets_Should_Return_TestSetModels()
    {
        // Arrange
        var testSets = new List<TestSet>
        {
            new()
            {
                Id = "000000000000000000000000",
                Inputs =
                [
                    new()
                    {
                        DataType = "Data Type #1",
                        Index = 1,
                        IsArray = true,
                        ValueAsJson = "ValueAsJson #1"
                    },
                    new()
                    {
                        DataType = "Data Type #2",
                        Index = 2,
                        IsArray = false,
                        ValueAsJson = "ValueAsJson #2"
                    },
                    new()
                    {
                        DataType = "Data Type #3",
                        Index = 3,
                        IsArray = true,
                        ValueAsJson = "ValueAsJson #3"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #1",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000000")
            },
            new()
            {
                Id = "000000000000000000000001",
                Inputs =
                [
                    new()
                    {
                        DataType = "Data Type #4",
                        Index = 1,
                        IsArray = true,
                        ValueAsJson = "ValueAsJson #4"
                    },
                    new()
                    {
                        DataType = "Data Type #5",
                        Index = 2,
                        IsArray = false,
                        ValueAsJson = "ValueAsJson #5"
                    },
                    new()
                    {
                        DataType = "Data Type #6",
                        Index = 3,
                        IsArray = true,
                        ValueAsJson = "ValueAsJson #6"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #2",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000000")
            }
        };

        // Act
        var testSetModels = testSets.ToModels();

        foreach (var testSetModel in testSetModels)
        {
            var testSet = testSets.SingleOrDefault(_ => _.Id == testSetModel.Id);

            Assert.NotNull(testSet);

            // Assert
            Assert.Equal(testSet.Id, testSetModel.Id);
            Assert.NotNull(testSetModel.Inputs);
            Assert.NotEmpty(testSetModel.Inputs);

            foreach (var testSetInput in testSet.Inputs!)
            {
                var testSetInputModel = testSetModel.Inputs.SingleOrDefault(_ => _.Index == testSetInput.Index);

                Assert.NotNull(testSetInputModel);
                Assert.Equal(testSetInput.DataType, testSetInputModel.DataType);
                Assert.Equal(testSetInput.IsArray, testSetInputModel.IsArray);
                Assert.Equal(testSetInput.ValueAsJson, testSetInputModel.ValueAsJson);
            }

            Assert.Equal(testSet.Name, testSetModel.Name);
            Assert.Equal(testSet.Problem?.Id.AsString, testSetModel.ProblemId);
        }
    }
}
