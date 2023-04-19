namespace Pork.Shared.Entities.Messages.Site;

public class SiteMessage {
    public int Id { get; set; }

    public Guid? FlowId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public int SiteId { get; set; }
    public Entities.Site Site { get; set; }
}