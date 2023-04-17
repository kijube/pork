using Pork.Shared.Entities;

namespace Pork.Manager.Dtos;

public class LocalClientDto {
    public required int Id { get; init; }
    public required GlobalClientDto GlobalClient { get; init; }
    public required SiteNameDto Site { get; init; }
    public required bool IsOnline { get; init; }
    public required DateTimeOffset LastSeen { get; init; }

    public static LocalClientDto From(LocalClient localClient) {
        return new LocalClientDto
        {
            Id = localClient.Id,
            GlobalClient = GlobalClientDto.From(localClient.GlobalClient),
            Site = SiteNameDto.From(localClient.Site),
            IsOnline = localClient.IsOnline,
            LastSeen = localClient.LastSeen
        };
    }
}