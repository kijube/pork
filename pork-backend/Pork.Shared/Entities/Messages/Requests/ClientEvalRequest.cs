using Pork.DtoGenerator.Attributes;

namespace Pork.Shared.Entities.Messages.Requests;

[GenerateDto("Internal")]
[GenerateDto("External")]
public class ClientEvalRequest : ClientRequest {
    [DtoInclude("*")] public required string Code { get; init; }
}