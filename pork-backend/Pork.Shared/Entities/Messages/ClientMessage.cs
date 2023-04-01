using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pork.DtoGenerator.Attributes;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Shared.Entities.Messages;

[BsonKnownTypes(typeof(FailureClientResponse), typeof(ClientHookResponse), typeof(ClientEvalResponse),
    typeof(ClientEvalRequest))]
public class ClientMessage {
    public ObjectId Id { get; set; }

    [DtoInclude] public required string Type { get; init; }
    [DtoInclude("*")] public Guid? FlowId { get; init; }
    [DtoInclude("Internal")] public required DateTimeOffset Timestamp { get; init; }
    public required string ClientId { get; init; }
}