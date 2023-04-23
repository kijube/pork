namespace Pork.Shared.Entities.Messages.Responses;

public class ClientDumpResponse : ClientResponse {
    public required string Key { get; init; }
    public required string Dump { get; init; }
}