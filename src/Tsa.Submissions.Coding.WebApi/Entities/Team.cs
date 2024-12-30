using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class Team : IMongoDbEntity
{
    public CompetitionLevel CompetitionLevel { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [SwaggerSchema(ReadOnly = true)]
    public string? Id { get; set; }

    public IList<Participant> Participants { get; set; } = [];

    public string? SchoolNumber { get; set; }

    public string? TeamNumber { get; set; }
}
