using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pork.Manager.Dtos;
using Pork.Shared;

namespace Pork.Manager.Endpoints;

public static class SiteEndpointExtensions {
    public static void MapSiteEndpoints(this WebApplication app) {
        app.MapGroup("/sites")
            .MapGetAll();
    }

    private static IEndpointRouteBuilder MapGetAll(this IEndpointRouteBuilder group) {
        group.MapGet("",
                async ([FromServices] DataContext dataContext) =>
                    (await dataContext.Sites.ToListAsync())
                    .Select(SiteDto.From))
            .WithName("GetSites");
        return group;
    }
}