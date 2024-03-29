﻿using Pork.Shared.Entities;

namespace Pork.Manager.Dtos;

public class SiteDto {
    public int Id { get; init; }
    public required string Key { get; init; }

    public required List<LocalClientDto> LocalClients { get; init; }

    public static SiteDto From(Site site) {
        return new SiteDto
        {
            Id = site.Id,
            Key = site.Key,
            LocalClients = site.LocalClients.Select(LocalClientDto.From).ToList()
        };
    }
}