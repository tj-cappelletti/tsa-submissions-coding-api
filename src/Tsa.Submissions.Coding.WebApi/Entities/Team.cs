using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class Team
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [SwaggerSchema(ReadOnly = true)]
    public string Id { get; set; }

    public IList<Participant> Participants { get; set; }

    public string SchoolNumber { get; set; }

    public string TeamId => $"{SchoolNumber}-{TeamNumber}";

    public string TeamNumber { get; set; }

    public Team()
    {
        Participants = new List<Participant>();
    }

    public override string ToString()
    {
        return TeamId;
    }
}
