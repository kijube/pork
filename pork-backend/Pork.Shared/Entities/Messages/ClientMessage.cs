using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Shared.Entities.Messages;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(ClientRequest), typeof(ClientResponse))]
public class ClientMessage {
    public ObjectId Id { get; set; }

    public Guid? FlowId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string ClientId { get; set; }
    
    [BsonElement]
    public virtual bool ShowInConsole => false;
}