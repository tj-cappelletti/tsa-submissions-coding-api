﻿using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Tsa.Submissions.Coding.UnitTests.ExtensionMethods;

[ExcludeFromCodeCoverage]
public static class FilterDefinitionExtensions
{
    public static string RenderToJson<TDocument>(this FilterDefinition<TDocument> filter)
    {
        var serializerRegistry = BsonSerializer.SerializerRegistry;
        var documentSerializer = serializerRegistry.GetSerializer<TDocument>();
        return filter.Render(documentSerializer, serializerRegistry).ToJson();
    }
}
