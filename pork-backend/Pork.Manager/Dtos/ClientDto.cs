using Pork.Shared.Entities;

namespace Pork.Manager.Dtos;

public class ClientDto {
    public required string ClientId { get; init; }
    public required string? RemoteIp { get; init; }
    public required bool IsOnline { get; init; }
    public required DateTimeOffset LastSeen { get; init; }
    public required string? Nickname { get; init; }

    public static ClientDto From(Client client) {
        return new ClientDto
        {
            ClientId = client.ClientId,
            RemoteIp = client.RemoteIp,
            IsOnline = client.IsOnline,
            LastSeen = client.LastSeen,
            Nickname = client.Nickname
        };
    }
}