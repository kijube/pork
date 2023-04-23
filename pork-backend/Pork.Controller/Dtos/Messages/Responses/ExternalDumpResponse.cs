namespace Pork.Controller.Dtos.Messages.Responses;

public class ExternalDumpResponse : ExternalResponse {
    public required string Key { get; init; }
    public required string Dump { get; init; }
}