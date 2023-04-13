using Pork.Shared.Entities;

namespace Pork.Manager.Dtos;

public class ClientDto {
    public required Guid ClientId { get; init; }
    public required string? RemoteIp { get; init; }
    public required bool IsOnline { get; init; }
    public required DateTimeOffset LastSeen { get; init; }
    public required string? Nickname { get; init; }

    public static ClientDto From(GlobalClient globalClient) {
        return new ClientDto
        {
            ClientId = globalClient.Id,
            RemoteIp = globalClient.RemoteIp,
            IsOnline = globalClient.IsOnline,
            LastSeen = globalClient.LastSeen,
            Nickname = globalClient.Nickname
        };
    }
}