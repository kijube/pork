namespace Pork.Controller.Dtos.Messages.Responses;

public class ExternalHookResponse : ExternalResponse {
    public required string Method { get; init; }
    public required string HookId { get; init; }
    public string? Args { get; set; }
    public string? Result { get; set; }
}