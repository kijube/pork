namespace Pork.Shared.Entities.Messages.Responses;

public class ClientResponse : ClientMessage {
    public required bool Success { get; init; }
}