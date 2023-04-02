namespace Pork.Controller.Dtos.Messages.Requests;

public class ExternalEvalRequest : ExternalRequest {
    public required string Code { get; init; }
}