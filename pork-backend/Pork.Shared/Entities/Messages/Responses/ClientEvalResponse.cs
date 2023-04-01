namespace Pork.Shared.Entities.Messages.Responses;

public class ClientEvalResponse : SuccessClientResponse {
    public required string Data { get; set; }
}