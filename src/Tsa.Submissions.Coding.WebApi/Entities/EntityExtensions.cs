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

    public static List<ParticipantModel> ToModels(this IList<Participant> participants)
    {
        return participants.Select(participant => participant.ToModel()).ToList();
    }

    public static List<TeamModel> ToModels(this IList<Team> teams)
    {
        return teams.Select(team => team.ToModel()).ToList();
    }

    public static List<ProblemModel> ToModels(this IList<Problem> problems)
    {
        return problems.Select(problem => problem.ToModel()).ToList();
    }
}
