using Pork.Manager.Dtos.Messages.Responses;

namespace Pork.Manager.Dtos.Messages.Requests;

public class InternalEvalRequest : InternalRequest {
    public required string Code { get; init; }
    public required InternalEvalResponse? Response { get; init; }
}