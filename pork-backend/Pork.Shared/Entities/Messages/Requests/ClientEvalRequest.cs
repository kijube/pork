namespace Pork.Shared.Entities.Messages.Requests;

public class ClientEvalRequest : ClientRequest {
    public required string Code { get; init; }
}