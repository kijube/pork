namespace Pork.Manager.Dtos.Messages.Requests;

public class InternalEvalRequest : InternalRequest {
    public required string Code { get; init; }
}