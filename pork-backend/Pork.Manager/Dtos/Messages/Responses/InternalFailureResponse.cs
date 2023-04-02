namespace Pork.Manager.Dtos.Messages.Responses;

public class InternalFailureResponse : InternalResponse {
    public required string Error { get; init; }
}