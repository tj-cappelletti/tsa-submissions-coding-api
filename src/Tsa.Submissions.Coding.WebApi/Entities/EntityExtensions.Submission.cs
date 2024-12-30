using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
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

    public static List<SubmissionModel> ToModels(this IList<Submission> submissions)
    {
        return submissions.Select(submission => submission.ToModel()).ToList();
    }
}
