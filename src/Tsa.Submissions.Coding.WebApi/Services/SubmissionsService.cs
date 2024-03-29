﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class SubmissionsService : MongoDbService<Submission>, ISubmissionsService
{
    public const string MongoDbCollectionName = "submissions";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "Submissions";

    public SubmissionsService(IMongoClient mongoClient, IOptions<SubmissionsDatabase> options) : base(
        mongoClient,
        options.Value.DatabaseName,
        MongoDbCollectionName)
    { }
}
