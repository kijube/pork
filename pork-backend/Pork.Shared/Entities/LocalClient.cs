namespace Pork.Shared.Entities;

public class LocalClient {
    public int Id { get; set; }
    public Guid GlobalClientId { get; init; }
    public required GlobalClient GlobalClient { get; init; }
    public bool IsOnline { get; set; }
    public int SiteId { get; init; }
    public required Site Site { get; init; }
    public DateTimeOffset LastSeen { get; set; }
}