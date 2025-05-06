using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static partial class ModelExtensions
{
    public static Submission ToEntity(this SubmissionModel submissionModel)
    {
        return new Submission
        {
            Id = submissionModel.Id,
            IsFinalSubmission = submissionModel.IsFinalSubmission,
            Language = submissionModel.Language,
            Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, submissionModel.ProblemId),
            Solution = submissionModel.Solution,
            SubmittedOn = submissionModel.SubmittedOn,
            Team = new MongoDBRef(TeamsService.MongoDbCollectionName, submissionModel.TeamId),
            TestSetResults = submissionModel.TestSetResults.ToEntities()
        };
    }
}
