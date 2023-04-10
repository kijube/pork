namespace Pork.Shared.Entities.Messages.Requests;

public abstract class ClientRequest : ClientMessage {
    public bool Sent { get; set; }
    public DateTimeOffset? SentAt { get; set; }
}