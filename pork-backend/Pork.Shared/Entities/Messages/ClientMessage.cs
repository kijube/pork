namespace Pork.Shared.Entities.Messages;

public abstract class ClientMessage {
    public int Id { get; set; }

    public Guid? FlowId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public int LocalClientId { get; set; }
    public LocalClient LocalClient { get; set; }
    public bool ShowInConsole { get; set; } = false;
}