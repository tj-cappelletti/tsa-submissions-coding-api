﻿using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public abstract class MongoDbService<T> where T : IMongoDbEntity
{
    protected IMongoCollection<T> EntityCollection;
    protected IMongoDatabase MongoDatabase;

    protected MongoDbService(IMongoClient mongoClient, string databaseName, string collectionName)
    {
        MongoDatabase = mongoClient.GetDatabase(databaseName);

        EntityCollection = MongoDatabase.GetCollection<T>(collectionName);
    }

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await EntityCollection.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<T>.Filter.Eq(_ => _.Id, id);

        var cursor = await EntityCollection.FindAsync(filterDefinition, null, cancellationToken);

        return await cursor.AnyAsync(cancellationToken);
    }

    public async Task<List<T>> GetAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await EntityCollection.FindAsync(Builders<T>.Filter.Empty, null, cancellationToken);

        var result = await cursor.ToListAsync(cancellationToken);

        return result;
    }

    public async Task<List<T>> GetAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<T>.Filter.In(_ => _.Id, ids);

        var cursor = await EntityCollection.FindAsync(filterDefinition, null, cancellationToken);

        var result = await cursor.ToListAsync(cancellationToken);

        return result;
    }

    public async Task<T?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<T>.Filter.Eq(_ => _.Id, id);

        var cursor = await EntityCollection.FindAsync(filterDefinition, null, cancellationToken);

        var result = await cursor.FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<bool> PingAsync(CancellationToken cancellationToken = default)
    {
        var successful = true;

        try
        {
            await EntityCollection.Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}", cancellationToken: cancellationToken);
        }
        catch (Exception)
        {
            successful = false;
        }

        return successful;
    }

    public async Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<T>.Filter.Eq(_ => _.Id, entity.Id);

        await EntityCollection.DeleteOneAsync(filterDefinition, null, cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        var filterDefinition = Builders<T>.Filter.Eq(_ => _.Id, entity.Id);

        var replaceOptions = new ReplaceOptions
        {
            IsUpsert = false
        };

        await EntityCollection.ReplaceOneAsync(filterDefinition, entity, replaceOptions, cancellationToken);
    }
}
