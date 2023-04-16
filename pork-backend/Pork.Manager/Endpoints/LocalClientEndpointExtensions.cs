using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pork.Manager.Dtos;
using Pork.Manager.Dtos.Messages.Requests;
using Pork.Shared;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Manager.Endpoints;

public static class LocalClientEndpointExtensions {
    public static void MapLocalClientEndpoints(this WebApplication app) {
        var group = app.MapGroup("/clients/local")
            .MapGetAll();

        var clientGroup = group.MapGroup("/{localClientId}")
            .MapGetById()
            .MapGetLogs()
            .MapGetEvents()
            .MapGetConsoleEvents();

        var actionsGroup = clientGroup.MapGroup("/actions")
            .MapRunEval();
    }

    private static IEndpointRouteBuilder MapRunEval(this IEndpointRouteBuilder group) {
        group.MapPost("/eval",
                async ([FromServices] DataContext dataContext, int localClientId,
                    [FromBody] EvalRequestDto request) => {
                    var client = await dataContext.LocalClients.FirstOrDefaultAsync(c => c.Id == localClientId);
                    if (client == null) {
                        return Results.NotFound();
                    }

                    var eval = new ClientEvalRequest
                    {
                        Timestamp = DateTimeOffset.UtcNow,
                        LocalClientId = client.Id,
                        Code = request.Code,
                        FlowId = Guid.NewGuid(),
                    };

                    await dataContext.ClientMessages.AddAsync(eval);
                    await dataContext.SaveChangesAsync();
                    return Results.Ok(DtoMapper.MapRequest(eval) as InternalEvalRequest);
                })
            .Produces<InternalEvalRequest>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("RunClientEval");
        return group;
    }

    private static IEndpointRouteBuilder MapGetEvents(this IEndpointRouteBuilder group) {
        group.MapGet("/events",
                async ([FromServices] DataContext dataContext, int localClientId, [FromQuery] int count,
                        [FromQuery] int offset) =>
                    (await dataContext.ClientMessages.Where(e => e.LocalClientId == localClientId)
                        .OrderByDescending(e => e.Timestamp)
                        .Skip(offset)
                        .Take(count)
                        .ToListAsync())
                    .Select(DtoMapper.MapMessage))
            .WithName("GetLocalClientEvents");
        return group;
    }

    private static IEndpointRouteBuilder MapGetConsoleEvents(this IEndpointRouteBuilder group) {
        group.MapGet("/events/console",
                async ([FromServices] DataContext dataContext, int localClientId, [FromQuery] int count,
                    [FromQuery] int offset) => {
                    var requests = await dataContext.ClientMessages
                        .Where(e => e.LocalClientId == localClientId &&
                                    e.ShowInConsole)
                        .OrderBy(e => e.Timestamp)
                        .Skip(offset)
                        .Take(count)
                        .ToListAsync();

                    // todo improve on this, this is terrible
                    foreach (var r in requests.OfType<ClientEvalRequest>()) {
                        if (r.ResponseId is not null) {
                            r.Response = await dataContext.ClientMessages.OfType<ClientEvalResponse>()
                                .FirstOrDefaultAsync(e => e.FlowId == r.FlowId);
                        }
                    }

                    return requests.Select(DtoMapper.MapMessage);
                })
            .WithName("GetClientConsoleEvents");
        return group;
    }

    private static IEndpointRouteBuilder MapGetById(this IEndpointRouteBuilder group) {
        group.MapGet("", async ([FromServices] DataContext dataContext, int localClientId) => {
                var client = await dataContext.LocalClients.Include(l => l.GlobalClient)
                    .Include(l => l.Site)
                    .FirstOrDefaultAsync(e => e.Id == localClientId);
                return client is null ? Results.NotFound() : Results.Ok(LocalClientDto.From(client));
            })
            .Produces<LocalClientDto>()
            .WithName("GetLocalClientById");
        return group;
    }

    private static IEndpointRouteBuilder MapGetLogs(this IEndpointRouteBuilder group) {
        group.MapGet("/logs",
                async ([FromServices] DataContext dataContext, int localClientId,
                        [FromQuery] [Range(1, 100)] int count,
                        [FromQuery] int offset) =>
                    (await dataContext.ClientLogs.Where(e => e.LocalClientId == localClientId)
                        .OrderByDescending(e => e.Timestamp)
                        .Skip(offset)
                        .Take(count)
                        .ToListAsync()).Select(ClientLogDto.From))
            .WithName("GetClientLogs");
        return group;
    }

    private static IEndpointRouteBuilder MapGetAll(this IEndpointRouteBuilder group) {
        group.MapGet("/",
                async ([FromServices] DataContext dataContext) =>
                    (await dataContext.LocalClients.Include(l => l.GlobalClient).Include(l => l.Site).ToListAsync())
                    .Select(
                        LocalClientDto.From))
            .WithName("GetLocalClients");
        return group;
    }
}