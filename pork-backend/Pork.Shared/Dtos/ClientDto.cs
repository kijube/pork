using Pork.DtoGenerator;
using Pork.DtoGenerator.Attributes;
using Pork.Shared.Entities;

namespace Pork.Shared.Dtos;

public class ClientDto {
    public required string ClientId { get; init; }
    public string? RemoteIp { get; init; }
    public bool IsOnline { get; init; }
    public DateTimeOffset LastSeen { get; init; }
    public string? Nickname { get; init; }

    public static ClientDto MapFrom(Client client) {
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