using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static class ModelExtensions
{
    public static List<Participant> ToEntities(this IList<ParticipantModel> participants)
    {
        return participants.Select(participant => participant.ToEntity()).ToList();
    }

    public static Participant ToEntity(this ParticipantModel participantModel)
    {
        return new Participant
        {
            ParticipantNumber = participantModel.ParticipantNumber,
            SchoolNumber = participantModel.SchoolNumber
        };
    }

    public static Team ToEntity(this TeamModel teamModel)
    {
        return new Team
        {
            Id = teamModel.Id,
            Participants = teamModel.Participants.ToEntities(),
            SchoolNumber = teamModel.SchoolNumber,
            TeamNumber = teamModel.TeamNumber
        };
    }

    public static Problem ToEntity(this ProblemModel problemModel)
    {
        return new Problem
        {
            Description = problemModel.Description,
            Id = problemModel.Id,
            IsActive = problemModel.IsActive
        };
    }
}
