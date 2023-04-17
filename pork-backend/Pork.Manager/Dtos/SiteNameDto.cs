using Pork.Shared.Entities;

namespace Pork.Manager.Dtos;

public class SiteNameDto {
    public int Id { get; init; }
    public required string Key { get; init; }

    public static SiteNameDto From(Site site) {
        return new SiteNameDto
        {
            Id = site.Id,
            Key = site.Key
        };
    }
}