using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class Team
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string SchoolNumber { get; set; }

    public string TeamNumber { get; set; }

    public string TeamId => $"{SchoolNumber}-{TeamNumber}";

    public IList<Participant> Participants { get; set; }

    public Team()
    {
        Participants = new List<Participant>();
    }
}
