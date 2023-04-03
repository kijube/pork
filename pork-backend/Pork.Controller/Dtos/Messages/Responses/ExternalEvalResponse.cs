using System.Text.Json;

namespace Pork.Controller.Dtos.Messages.Responses;

public class ExternalEvalResponse : ExternalResponse {
    public required JsonElement? Data { get; init; }
}