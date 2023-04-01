using Pork.Shared.Entities;

namespace Pork.Shared.Dtos;

public class ClientLogDto {
    public required string ClientId { get; init; }
    public required string Level { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string Message { get; init; }

    public static ClientLogDto MapFrom(ClientLog clientLog) {
        return new ClientLogDto
        {
            ClientId = clientLog.ClientId,
            Level = clientLog.Level,
            Timestamp = clientLog.Timestamp,
            Message = clientLog.Message
        };
    }
}