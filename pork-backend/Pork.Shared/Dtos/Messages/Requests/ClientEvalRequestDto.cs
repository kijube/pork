namespace Pork.Shared.Dtos.Messages.Requests;

public class ClientEvalRequestDto : ClientRequestDto {
    public required string Code { get; init; }
}