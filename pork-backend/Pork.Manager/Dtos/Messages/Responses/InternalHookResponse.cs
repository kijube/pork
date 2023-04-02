namespace Pork.Manager.Dtos.Messages.Responses;

public class InternalHookResponse : InternalResponse {
    public required string Method { get; init; }
    public required string HookId { get; init; }
    public string? Args { get; set; }
    public string? Result { get; set; }
}