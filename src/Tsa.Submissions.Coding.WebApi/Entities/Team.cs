using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Team : IMongoDbEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [SwaggerSchema(ReadOnly = true)]
    public string? Id { get; set; }

    public IList<Participant> Participants { get; set; }

    public string? SchoolNumber { get; set; }

    public string? TeamNumber { get; set; }

    public Team()
    {
        Participants = new List<Participant>();
    }
}
