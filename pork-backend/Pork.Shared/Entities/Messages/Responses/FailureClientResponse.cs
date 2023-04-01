namespace Pork.Shared.Entities.Messages.Responses;

public class FailureClientResponse : ClientResponse {
    public required string Error { get; init; }

    public FailureClientResponse() {
        Success = false;
    }
}