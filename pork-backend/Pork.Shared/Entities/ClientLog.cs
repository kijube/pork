using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pork.DtoGenerator.Attributes;

namespace Pork.Shared.Entities;

[GenerateDto]
public class ClientLog {
    [BsonId] public ObjectId Id { get; init; }
    public required string ClientId { get; init; }
    [DtoInclude] public required string Level { get; init; }
    [DtoInclude] public required DateTimeOffset Timestamp { get; init; }
    [DtoInclude] public required string Message { get; init; }
}