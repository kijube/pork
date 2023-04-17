using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pork.Manager.Dtos;
using Pork.Shared;

namespace Pork.Manager.Endpoints;

public static class SiteEndpointExtensions {
    public static void MapSiteEndpoints(this WebApplication app) {
        app.MapGroup("/sites")
            .MapGetAll()
            .MapGetByKey();
    }

    private static IEndpointRouteBuilder MapGetAll(this IEndpointRouteBuilder group) {
        group.MapGet("",
                async ([FromServices] DataContext dataContext) =>
                    (await dataContext.Sites
                        .Include(s => s.LocalClients)
                        .ThenInclude(s => s.GlobalClient)
                        .ToListAsync())
                    .Select(SiteDto.From))
            .WithName("GetSites");
        return group;
    }

    public static IEndpointRouteBuilder MapGetByKey(this IEndpointRouteBuilder group) {
        group.MapGet("{siteKey}",
                async ([FromServices] DataContext dataContext, string siteKey) => {
                    var site = await dataContext.Sites
                        .Include(s => s.LocalClients)
                        .ThenInclude(s => s.GlobalClient)
                        .FirstOrDefaultAsync(s => s.Key == siteKey);
                    return site == null ? Results.NotFound() : Results.Ok(SiteDto.From(site));
                })
            .Produces<SiteDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetSiteByKey");
        return group;
    }
}