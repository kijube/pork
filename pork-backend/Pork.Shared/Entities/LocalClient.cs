namespace Pork.Shared.Entities;

public class LocalClient {
    public int Id { get; set; }

    public Guid GlobalClientId { get; init; }
    public required GlobalClient GlobalClient { get; init; }

    public required bool IsOnline { get; set; }
    public int SiteId { get; init; }
    public required Site Site { get; init; }
    public required DateTimeOffset LastSeen { get; set; }
    public string? RemoteIp { get; set; }
}