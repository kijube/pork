namespace Pork.Manager.Dtos.Messages;

public class InternalMessage {
    public string Type { get; set; }
    public Guid? FlowId { get; set; }
}