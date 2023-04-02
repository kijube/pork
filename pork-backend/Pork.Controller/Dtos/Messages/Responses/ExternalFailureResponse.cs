namespace Pork.Controller.Dtos.Messages.Responses;

public class ExternalFailureResponse : ExternalResponse {
    public required string Error { get; init; }
}