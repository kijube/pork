namespace Pork.Shared.Entities.Messages.Responses;

public class ClientFailureResponse : ClientResponse {
    public required string Error { get; init; }
}