using Pork.Shared.Entities;

namespace Pork.Manager.Dtos;

public class GlobalClientDto {
    public required Guid Id { get; init; }
    public required string? RemoteIp { get; init; }
    public required string? Nickname { get; init; }

    public static GlobalClientDto From(GlobalClient globalClient) {
        return new GlobalClientDto
        {
            Id = globalClient.Id,
            RemoteIp = globalClient.RemoteIp,
            Nickname = globalClient.Nickname
        };
    }
}