using Pork.Shared.Entities;

namespace Pork.Manager.Dtos;

public class ClientLogDto {
    public required Guid ClientId { get; init; }
    public required string Level { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string Message { get; init; }

    public static ClientLogDto From(ClientLog log) {
        return new ClientLogDto
        {
            ClientId = log.ClientId,
            Level = log.Level,
            Timestamp = log.Timestamp,
            Message = log.Message
        };
    }
}