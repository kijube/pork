namespace Pork.Controller.Dtos.Messages;

public class ExternalMessage {
    public string Type { get; set; }
    public Guid? FlowId { get; set; }
}