using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class Participant
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string ParticipantNumber { get; set; }

    public string SchoolNumber { get; set; }

    public string ParticipantId => $"{SchoolNumber}-{ParticipantNumber}";
}
