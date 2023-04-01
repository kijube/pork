using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pork.Shared.Entities;

public class ClientLog {
    [BsonId] public ObjectId Id { get; init; }
    public required string ClientId { get; init; }
    public required string Level { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string Message { get; init; }
}