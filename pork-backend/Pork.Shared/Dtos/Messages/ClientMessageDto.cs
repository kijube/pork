using System.Text.Json.Serialization;
using Pork.Shared.Dtos.Messages.Requests;
using Pork.Shared.Entities.Messages;

namespace Pork.Shared.Dtos.Messages;

[JsonDerivedType(typeof(ClientEvalRequestDto))]
public abstract class ClientMessageDto {
    public string Type { get; set; }
    public Guid? FlowId { get; set; }
    public DateTimeOffset Timestamp { get; init; }
    public string ClientId { get; init; }

    public static ClientMessageDto MapFrom(ClientMessage clientMessage) {
    }
}