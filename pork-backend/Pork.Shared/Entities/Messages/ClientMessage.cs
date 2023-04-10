namespace Pork.Shared.Entities.Messages;

public abstract class ClientMessage {
    public int Id { get; set; }

    public Guid? FlowId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public Guid ClientId { get; set; }
    public virtual bool ShowInConsole { get; set; } = false;

}