using System.Text.Json;

namespace Pork.Controller.Dtos.Messages.Responses;

public class ExternalHookResponse : ExternalResponse {
    public required string Method { get; init; }
    public required string HookId { get; init; }
    public JsonElement? Args { get; set; }
    public JsonElement? Result { get; set; }
}