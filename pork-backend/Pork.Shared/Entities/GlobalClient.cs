namespace Pork.Shared.Entities;

public class GlobalClient {
    public required Guid Id { get; init; }
    public string? RemoteIp { get; set; }
    public string? Nickname { get; set; }
}