using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
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

        var expectedSubmissionModel = new SubmissionModel
        {
            Id = submission.Id,
            IsFinalSubmission = submission.IsFinalSubmission,
            Language = submission.Language,
            ProblemId = submission.Problem?.Id.AsString,
            Solution = submission.Solution,
            SubmittedOn = submission.SubmittedOn,
            TeamId = submission.Team?.Id.AsString
        };

        if (includeTestTests)
        {
            expectedSubmissionModel.TestSetResults = new List<TestSetResultModel>();
            submission.TestSetResults =
            [
                new TestSetResult
                {
                    Passed = true,
                    RunDuration = new TimeSpan(0, 0, 5, 0),
                    TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
                },
                new TestSetResult
                {
                    Passed = true,
                    RunDuration = new TimeSpan(0, 0, 5, 0),
                    TestSet = new MongoDBRef("test-sets", "000000000000000000000011")
                }
            ];

            foreach (var submissionTestSetResult in submission.TestSetResults)
            {
                expectedSubmissionModel.TestSetResults.Add(new TestSetResultModel
                {
                    Passed = submissionTestSetResult.Passed,
                    RunDuration = submissionTestSetResult.RunDuration,
                    TestSetId = submissionTestSetResult.TestSet?.Id.AsString
                });
            }
        }

        // Act
        var actualSubmissionModel = submission.ToModel();

        // Assert
        Assert.Equal(expectedSubmissionModel, actualSubmissionModel, new SubmissionModelEqualityComparer());
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

        var expectedParticipantModel = new ParticipantModel
        {
            ParticipantNumber = participant.ParticipantNumber,
            SchoolNumber = participant.SchoolNumber
        };

        // Act
        var actualParticipantModel = participant.ToModel();

        // Assert
        Assert.Equal(expectedParticipantModel, actualParticipantModel, new ParticipantModelEqualityComparer());
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

        var expectedProblemModel = new ProblemModel
        {
            Description = problem.Description,
            Id = problem.Id,
            IsActive = problem.IsActive,
            Title = problem.Title
        };

        // Act
        var actualProblemModel = problem.ToModel();

        // Assert
        Assert.Equal(expectedProblemModel, actualProblemModel, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Team_Should_Return_TeamModel()
    {
        // Arrange
        var team = new Team
        {
            CompetitionLevel = CompetitionLevel.HighSchool,
            Id = "This is an ID",
            Participants =
            [
                new Participant
                {
                    ParticipantNumber = "001",
                    SchoolNumber = "1234"
                },
                new Participant
                {
                    ParticipantNumber = "002",
                    SchoolNumber = "1234"
                }
            ],
            SchoolNumber = "1234",
            TeamNumber = "901"
        };

        var expectedTeamModel = new TeamModel
        {
            CompetitionLevel = team.CompetitionLevel.ToString(),
            Id = team.Id,
            Participants = [],
            SchoolNumber = team.SchoolNumber,
            TeamNumber = team.TeamNumber
        };

        foreach (var teamParticipant in team.Participants)
        {
            expectedTeamModel.Participants.Add(new ParticipantModel
            {
                ParticipantNumber = teamParticipant.ParticipantNumber,
                SchoolNumber = teamParticipant.SchoolNumber
            });
        }

        // Act
        var actualTeamModel = team.ToModel();

        // Assert
        Assert.Equal(expectedTeamModel, actualTeamModel, new TeamModelEqualityComparer());
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
                new TestSetValue
                {
                    DataType = "Data Type #1",
                    Index = 1,
                    IsArray = true,
                    ValueAsJson = "ValueAsJson #1"
                },
                new TestSetValue
                {
                    DataType = "Data Type #2",
                    Index = 2,
                    IsArray = false,
                    ValueAsJson = "ValueAsJson #2"
                },
                new TestSetValue
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

        var expectedTestSetModel = new TestSetModel
        {
            Id = testSet.Id,
            Inputs = [],
            IsPublic = testSet.IsPublic,
            Name = testSet.Name,
            ProblemId = testSet.Problem?.Id.AsString
        };

        foreach (var testSetInput in testSet.Inputs)
        {
            expectedTestSetModel.Inputs.Add(new TestSetValueModel
            {
                DataType = testSetInput.DataType,
                Index = testSetInput.Index,
                IsArray = testSetInput.IsArray,
                ValueAsJson = testSetInput.ValueAsJson
            });
        }

        // Act
        var actualTestSetModel = testSet.ToModel();

        // Assert
        Assert.Equal(expectedTestSetModel, actualTestSetModel, new TestSetModelEqualityComparer());
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
                new TestSetValue
                {
                    DataType = "Data Type #1",
                    Index = 1,
                    ValueAsJson = "ValueAsJson #1"
                },
                new TestSetValue
                {
                    DataType = "Data Type #2",
                    Index = 2,
                    ValueAsJson = "ValueAsJson #2"
                },
                new TestSetValue
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

        var expectedTestSetModel = new TestSetModel
        {
            Id = testSet.Id,
            Inputs = [],
            IsPublic = testSet.IsPublic,
            Name = testSet.Name,
            ProblemId = null
        };

        foreach (var testSetInput in testSet.Inputs)
        {
            expectedTestSetModel.Inputs.Add(new TestSetValueModel
            {
                DataType = testSetInput.DataType,
                Index = testSetInput.Index,
                IsArray = testSetInput.IsArray,
                ValueAsJson = testSetInput.ValueAsJson
            });
        }

        // Act
        var actualTestSetModel = testSet.ToModel();

        // Assert
        Assert.Equal(expectedTestSetModel, actualTestSetModel, new TestSetModelEqualityComparer());
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

        var expectedTestSetInputModel = new TestSetValueModel
        {
            DataType = testSetInput.DataType,
            Index = testSetInput.Index,
            IsArray = testSetInput.IsArray,
            ValueAsJson = testSetInput.ValueAsJson
        };

        // Act
        var actualTestSetInputModel = testSetInput.ToModel();

        // Assert
        Assert.Equal(expectedTestSetInputModel, actualTestSetInputModel, new TestSetValueModelEqualityComparer());
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

        var expectedTestSetResultModel = new TestSetResultModel
        {
            Passed = testSetResult.Passed,
            RunDuration = testSetResult.RunDuration,
            TestSetId = testSetResult.TestSet?.Id.AsString
        };

        // Act
        var actualTestSetResultModel = testSetResult.ToModel();

        // Assert
        Assert.Equal(expectedTestSetResultModel, actualTestSetResultModel, new TestSetResultModelEqualityComparer());
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
                    new TestSetResult
                    {
                        Passed = true,
                        RunDuration = new TimeSpan(0, 0, 5, 0),
                        TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
                    },
                    new TestSetResult
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
                    new TestSetResult
                    {
                        Passed = true,
                        RunDuration = new TimeSpan(0, 0, 5, 0),
                        TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
                    },
                    new TestSetResult
                    {
                        Passed = true,
                        RunDuration = new TimeSpan(0, 0, 5, 0),
                        TestSet = new MongoDBRef("test-sets", "000000000000000000000011")
                    }
                ]
            }
        };

        var expectedSubmissionModels = new List<SubmissionModel>
        {
            new()
            {
                Id = submissions[0].Id,
                IsFinalSubmission = submissions[0].IsFinalSubmission,
                Language = submissions[0].Language,
                ProblemId = submissions[0].Problem?.Id.AsString,
                Solution = submissions[0].Solution,
                SubmittedOn = submissions[0].SubmittedOn,
                TeamId = submissions[0].Team?.Id.AsString,
                TestSetResults =
                [
                    new TestSetResultModel
                    {
                        Passed = submissions[0].TestSetResults![0].Passed,
                        RunDuration = submissions[0].TestSetResults![0].RunDuration,
                        TestSetId = submissions[0].TestSetResults![0].TestSet!.Id.AsString
                    },
                    new TestSetResultModel
                    {
                        Passed = submissions[0].TestSetResults![1].Passed,
                        RunDuration = submissions[0].TestSetResults![1].RunDuration,
                        TestSetId = submissions[0].TestSetResults![1].TestSet!.Id.AsString
                    }
                ]
            },
            new()
            {
                Id = submissions[1].Id,
                IsFinalSubmission = submissions[1].IsFinalSubmission,
                Language = submissions[1].Language,
                ProblemId = submissions[1].Problem?.Id.AsString,
                Solution = submissions[1].Solution,
                SubmittedOn = submissions[1].SubmittedOn,
                TeamId = submissions[1].Team?.Id.AsString,
                TestSetResults =
                [
                    new TestSetResultModel
                    {
                        Passed = submissions[1].TestSetResults![0].Passed,
                        RunDuration = submissions[1].TestSetResults![0].RunDuration,
                        TestSetId = submissions[1].TestSetResults![0].TestSet!.Id.AsString
                    },
                    new TestSetResultModel
                    {
                        Passed = submissions[1].TestSetResults![1].Passed,
                        RunDuration = submissions[1].TestSetResults![1].RunDuration,
                        TestSetId = submissions[1].TestSetResults![1].TestSet!.Id.AsString
                    }
                ]
            }
        };

        // Act
        var actualSubmissionModels = submissions.ToModels();

        // Assert
        Assert.Equal(expectedSubmissionModels, actualSubmissionModels, new SubmissionModelEqualityComparer());
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

        var expectedTestSetResultModels = new List<TestSetResultModel>
        {
            new()
            {
                Passed = testSetResults[0].Passed,
                RunDuration = testSetResults[0].RunDuration,
                TestSetId = testSetResults[0].TestSet?.Id.AsString
            },
            new()
            {
                Passed = testSetResults[1].Passed,
                RunDuration = testSetResults[1].RunDuration,
                TestSetId = testSetResults[1].TestSet?.Id.AsString
            }
        };

        // Act
        var actualTestSetResultModels = testSetResults.ToModels();

        // Assert
        Assert.Equal(expectedTestSetResultModels, actualTestSetResultModels, new TestSetResultModelEqualityComparer());
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
                    new TestSetValue
                    {
                        DataType = "Data Type #1",
                        Index = 1,
                        IsArray = true,
                        ValueAsJson = "ValueAsJson #1"
                    },
                    new TestSetValue
                    {
                        DataType = "Data Type #2",
                        Index = 2,
                        IsArray = false,
                        ValueAsJson = "ValueAsJson #2"
                    },
                    new TestSetValue
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
                    new TestSetValue
                    {
                        DataType = "Data Type #4",
                        Index = 1,
                        IsArray = true,
                        ValueAsJson = "ValueAsJson #4"
                    },
                    new TestSetValue
                    {
                        DataType = "Data Type #5",
                        Index = 2,
                        IsArray = false,
                        ValueAsJson = "ValueAsJson #5"
                    },
                    new TestSetValue
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

        var expectedTestSetModels = new List<TestSetModel>
        {
            new()
            {
                Id = testSets[0].Id,
                Inputs = [],
                IsPublic = testSets[0].IsPublic,
                Name = testSets[0].Name,
                ProblemId = testSets[0].Problem?.Id.AsString
            },
            new()
            {
                Id = testSets[1].Id,
                Inputs = [],
                IsPublic = testSets[1].IsPublic,
                Name = testSets[1].Name,
                ProblemId = testSets[1].Problem?.Id.AsString
            }
        };

        for (var index = 0; index < testSets.Count; index++)
        {
            var testSet = testSets[index];
            var expectedTestSetModel = expectedTestSetModels[index];

            expectedTestSetModel.Inputs = new List<TestSetValueModel>();

            foreach (var testSetInput in testSet.Inputs!)
            {
                expectedTestSetModel.Inputs.Add(new TestSetValueModel
                {
                    DataType = testSetInput.DataType,
                    Index = testSetInput.Index,
                    IsArray = testSetInput.IsArray,
                    ValueAsJson = testSetInput.ValueAsJson
                });
            }
        }

        // Act
        var actualTestSetModels = testSets.ToModels();

        // Assert
        Assert.Equal(expectedTestSetModels, actualTestSetModels, new TestSetModelEqualityComparer());
    }
}
