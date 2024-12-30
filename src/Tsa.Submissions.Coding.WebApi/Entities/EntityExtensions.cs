using System.Collections.Generic;
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
            // Problem is required, if null, we are in a bad state
            ProblemId = submission.Problem!.Id.AsString,
            Solution = submission.Solution,
            SubmittedOn = submission.SubmittedOn,
            // Team is required, if null, we are in a bad state
            TeamId = submission.Team!.Id.AsString,
            TestSetResults = submission.TestSetResults?.ToModels()
        };
    }

    public static TeamModel ToModel(this Team team)
    {
        return new TeamModel
        {
            CompetitionLevel = team.CompetitionLevel.ToString(),
            Id = team.Id,
            Participants = team.Participants.ToModels(),
            SchoolNumber = team.SchoolNumber,
            TeamNumber = team.TeamNumber
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
            // TestSet is required, if null, we are in a bad state
            TestSetId = testSetResult.TestSet!.Id.AsString
        };
    }

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

    public static UserModel ToModel(this User user)
    {
        var userModel = new UserModel
        {
            Id = user.Id,
            Role = user.Role,
            UserName = user.UserName
        };

        if (user.Team != null)
            userModel.Team = new TeamModel
            {
                Id = user.Team.Id.AsString
            };

        return userModel;
    }

    public static List<ParticipantModel> ToModels(this IList<Participant> participants)
    {
        return participants.Select(participant => participant.ToModel()).ToList();
    }

    public static List<ProblemModel> ToModels(this IList<Problem> problems)
    {
        return problems.Select(problem => problem.ToModel()).ToList();
    }

    public static List<SubmissionModel> ToModels(this IList<Submission> submissions)
    {
        return submissions.Select(submission => submission.ToModel()).ToList();
    }

    public static List<TeamModel> ToModels(this IList<Team> teams)
    {
        return teams.Select(team => team.ToModel()).ToList();
    }

    public static List<TestSetModel> ToModels(this IList<TestSet> testSets)
    {
        return testSets.Select(testSet => testSet.ToModel()).ToList();
    }

    public static List<TestSetResultModel>? ToModels(this IList<TestSetResult>? testSetResults)
    {
        return testSetResults?.Select(testSetResult => testSetResult.ToModel()).ToList();
    }

    public static List<TestSetValueModel>? ToModels(this IList<TestSetValue>? testSetInputs)
    {
        return testSetInputs?.Select(testSetInput => testSetInput.ToModel()).ToList();
    }

    public static List<UserModel> ToModels(this IList<User> users)
    {
        return users.Select(user => user.ToModel()).ToList();
    }
}
