namespace Pork.Controller.Dtos.Messages;

public class ExternalMessage {
    public required string Type { get; init; }
    public Guid? FlowId { get; init; }
}