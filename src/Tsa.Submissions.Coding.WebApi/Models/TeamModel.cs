﻿using System.Collections.Generic;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class TeamModel
{
    public string? Id { get; set; }

    public IList<ParticipantModel> Participants { get; set; }

    public string? SchoolNumber { get; set; }

    public string? TeamId => string.IsNullOrWhiteSpace(SchoolNumber) || string.IsNullOrWhiteSpace(TeamNumber)
        ? null
        : $"{SchoolNumber}-{TeamNumber}";

    public string? TeamNumber { get; set; }

    public TeamModel()
    {
        Participants = [];
    }
}
