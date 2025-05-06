using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public class User : IMongoDbEntity
{
    public int? ExternalId { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [SwaggerSchema(ReadOnly = true)]
    public string? Id { get; set; }

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public MongoDBRef? Team { get; set; }

    public string? UserName { get; set; }
}
