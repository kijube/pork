namespace Pork.Manager.Dtos.Messages.Requests;

public class InternalRequest : InternalMessage {
    public required bool Sent { get; init; }
    public required DateTimeOffset? SentAt { get; init; }
}