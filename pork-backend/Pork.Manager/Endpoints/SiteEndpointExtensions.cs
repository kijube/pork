using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pork.Manager.Dtos;
using Pork.Manager.Dtos.Messages.Site;
using Pork.Shared;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Site;

namespace Pork.Manager.Endpoints;

public static class SiteEndpointExtensions {
    public static void MapSiteEndpoints(this WebApplication app) {
        var group = app.MapGroup("/sites")
            .MapGetAll();
        
        var specificSite = group.MapGroup("/{siteId:int}")
            .MapGetByKey()
            .MapGetEvals();
        
        var actions = specificSite.MapGroup("/actions")
            .MapBroadcastEval();
    }

    private static IEndpointRouteBuilder MapBroadcastEval(this IEndpointRouteBuilder group) {
        group.MapPost("broadcast-eval",
                async ([FromServices] DataContext dataContext, int siteId,
                    [FromBody] BroadcastEvalRequestDto request) => {
                    var site = await dataContext.Sites
                        .Include(s => s.LocalClients)
                        .ThenInclude(s => s.GlobalClient)
                        .FirstOrDefaultAsync(s => s.Id == siteId);
                    if (site == null) return Results.NotFound();
                    var message = new SiteBroadcastMessage {
                        Code = request.Code,
                        Site = site,
                        SiteId = site.Id,
                        FlowId = Guid.NewGuid(),
                        Timestamp = DateTimeOffset.UtcNow
                    };

                    var clients = await dataContext.LocalClients.Where(c => c.SiteId == site.Id).Select(c => c.Id)
                        .ToListAsync();

                    await dataContext.SiteBroadcastMessages.AddAsync(message);
                    await dataContext.SaveChangesAsync(); // save broadcast

                    await dataContext.ClientEvalRequests.AddRangeAsync(clients.Select(id => new ClientEvalRequest {
                        Code = request.Code,
                        LocalClientId = id,
                        FlowId = message.FlowId,
                        Timestamp = message.Timestamp
                    })); // save eval requests for existing clients


                    await dataContext.SaveChangesAsync();
                    return Results.Ok();
                })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("BroadcastEval");
        return group;
    }

    private static IEndpointRouteBuilder MapGetEvals(this IEndpointRouteBuilder group) {
        group.MapGet("evals",
                async ([FromServices] DataContext dataContext, int siteId) => {
                    var evals = await dataContext.SiteBroadcastMessages
                        .Where(m => m.SiteId == siteId)
                        .ToListAsync();
                    return evals.Select(DtoMapper.MapMessage);
                })
            .Produces<List<InternalSiteBroadcastMessage>>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetSiteEvals");
        return group;
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

    private static IEndpointRouteBuilder MapGetByKey(this IEndpointRouteBuilder group) {
        group.MapGet("",
                async ([FromServices] DataContext dataContext, int siteId) => {
                    var site = await dataContext.Sites
                        .Include(s => s.LocalClients)
                        .ThenInclude(s => s.GlobalClient)
                        .FirstOrDefaultAsync(s => s.Id == siteId);
                    return site == null ? Results.NotFound() : Results.Ok();
                })
            .Produces<SiteDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetSiteByKey");
        return group;
    }
}