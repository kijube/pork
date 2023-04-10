namespace Pork.Shared.Entities;

public class Client {
    public int Id { get; init; }
    public required Guid ClientId { get; init; }
    public string? RemoteIp { get; set; }
    public bool IsOnline { get; set; }
    public DateTimeOffset LastSeen { get; set; }
    public string? Nickname { get; set; }
}