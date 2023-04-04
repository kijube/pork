using MongoDB.Bson.Serialization.Attributes;

namespace Pork.Shared.Entities.Messages.Responses;

[BsonKnownTypes(typeof(ClientEvalResponse), typeof(ClientFailureResponse), typeof(ClientHookResponse))]
public class ClientResponse : ClientMessage {
}