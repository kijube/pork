namespace Pork.Shared.Entities;

public class GlobalClient {
    public required Guid Id { get; init; }
    public string? Nickname { get; set; }
}