namespace Pork.Shared.Entities.Messages.Responses;

public class ClientHookResponse : ClientResponse {
    public required string Method { get; set; }
    public required string HookId { get; set; }
    public string? Args { get; set; }
    public string? Result { get; set; }
}