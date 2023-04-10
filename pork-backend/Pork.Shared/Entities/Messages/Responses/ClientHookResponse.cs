namespace Pork.Shared.Entities.Messages.Responses;

public class ClientHookResponse : ClientResponse {
    public required string Method { get; init; }
    public required string HookId { get; init; }
    public string? Args { get; init; }
    public string? Result { get; init; }
}