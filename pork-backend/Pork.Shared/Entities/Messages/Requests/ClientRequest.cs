using MongoDB.Bson.Serialization.Attributes;

namespace Pork.Shared.Entities.Messages.Requests;

[BsonKnownTypes(typeof(ClientEvalRequest))]
public class ClientRequest : ClientMessage {
    public bool Sent { get; set; }
    public DateTimeOffset? SentAt { get; set; }
}