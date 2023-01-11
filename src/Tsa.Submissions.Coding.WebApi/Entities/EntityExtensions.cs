﻿using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static class EntityExtensions
{
    public static ParticipantModel ToModel(this Participant participant)
    {
        return new ParticipantModel
        {
            ParticipantNumber = participant.ParticipantNumber,
            SchoolNumber = participant.SchoolNumber
        };
    }

    public static ProblemModel ToModel(this Problem problem)
    {
        return new ProblemModel
        {
            Description = problem.Description,
            Id = problem.Id,
            IsActive = problem.IsActive,
            Title = problem.Title
        };
    }

    public static SubmissionModel ToModel(this Submission submission)
    {
        return new SubmissionModel
        {
            Id = submission.Id,
            IsFinalSubmission = submission.IsFinalSubmission,
            Language = submission.Language,
            ProblemId = submission.Problem?.Id.AsString,
            Solution = submission.Solution,
            SubmittedOn = submission.SubmittedOn,
            TeamId = submission.Team?.Id.AsString,
            TestSetResults = submission.TestSetResults?.ToModels()
        };
    }

    public static TeamModel ToModel(this Team team)
    {
        return new TeamModel
        {
            Id = team.Id,
            Participants = team.Participants.ToModels(),
            SchoolNumber = team.SchoolNumber,
            TeamNumber = team.TeamNumber
        };
    }

    public static TestSetInputModel ToModel(this TestSetInput testSetInput)
    {
        return new TestSetInputModel
        {
            DataType = testSetInput.DataType,
            Index = testSetInput.Index,
            IsArray = testSetInput.IsArray,
            ValueAsJson = testSetInput.ValueAsJson
        };
    }

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

    public static TestSetResultModel ToModel(this TestSetResult testSetResult)
    {
        return new TestSetResultModel
        {
            Passed = testSetResult.Passed,
            RunDuration = testSetResult.RunDuration,
            TestSetId = testSetResult.TestSet?.Id.AsString
        };
    }

    public static IList<SubmissionModel> ToModels(this IList<Submission> submissions)
    {
        return submissions.Select(submission => submission.ToModel()).ToList();
    }

    public static IList<TestSetResultModel>? ToModels(this IList<TestSetResult>? testSetResults)
    {
        return testSetResults?.Select(testSetResult => testSetResult.ToModel()).ToList();
    }

    public static List<ParticipantModel> ToModels(this IList<Participant> participants)
    {
        return participants.Select(participant => participant.ToModel()).ToList();
    }

    public static List<ProblemModel> ToModels(this IList<Problem> problems)
    {
        return problems.Select(problem => problem.ToModel()).ToList();
    }

    public static List<TeamModel> ToModels(this IList<Team> teams)
    {
        return teams.Select(team => team.ToModel()).ToList();
    }

    public static List<TestSetInputModel>? ToModels(this IList<TestSetInput>? testSetInputs)
    {
        return testSetInputs?.Select(testSetInput => testSetInput.ToModel()).ToList();
    }

    public static List<TestSetModel> ToModels(this IList<TestSet> testSets)
    {
        return testSets.Select(testSet => testSet.ToModel()).ToList();
    }
}
