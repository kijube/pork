using Pork.DtoGenerator.Attributes;

namespace Pork.Shared.Entities.Messages.Requests;

public class ClientRequest : ClientMessage {
    [DtoInclude("Internal")] public bool Sent { get; set; }
    [DtoInclude("Internal")] public DateTimeOffset? SentAt { get; set; }
}