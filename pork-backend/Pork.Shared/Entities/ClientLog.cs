namespace Pork.Shared.Entities;

public class ClientLog {
    public int Id { get; init; }
    public required int LocalClientId { get; init; }
    public LocalClient LocalClient { get; init; }

    public required string Level { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string Message { get; init; }
}