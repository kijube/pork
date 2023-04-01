using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Shared.Entities.Messages;

[BsonKnownTypes(typeof(FailureClientResponse), typeof(ClientHookResponse), typeof(ClientEvalResponse),
    typeof(ClientEvalRequest))]
public class ClientMessage {
    public ObjectId Id { get; set; }

    public required string Type { get; init; }
    public Guid? FlowId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string ClientId { get; init; }
}