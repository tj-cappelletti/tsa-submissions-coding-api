﻿using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static class ModelExtensions
{
    public static IList<TestSetInput>? ToEntities(this IList<TestSetInputModel>? testSetInputModels)
    {
        return testSetInputModels?.Select(testSetInputModel => testSetInputModel.ToEntity()).ToList();
    }

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

    public static Problem ToEntity(this ProblemModel problemModel)
    {
        return new Problem
        {
            Description = problemModel.Description,
            Id = problemModel.Id,
            IsActive = problemModel.IsActive,
            Title = problemModel.Title
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

    public static TestSet ToEntity(this TestSetModel testSetModel)
    {
        return new TestSet
        {
            Id = testSetModel.Id,
            Inputs = testSetModel.Inputs.ToEntities(),
            IsPublic = testSetModel.IsPublic,
            Name = testSetModel.Name,
            Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, testSetModel.ProblemId)
        };
    }

    public static TestSetInput ToEntity(this TestSetInputModel testSetInputModel)
    {
        return new TestSetInput
        {
            DataType = testSetInputModel.DataType,
            Index = testSetInputModel.Index,
            Value = testSetInputModel.Value
        };
    }
}
