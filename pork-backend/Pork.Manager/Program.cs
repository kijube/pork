using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pork.Manager.Dtos;
using Pork.Manager.Dtos.Messages.Requests;
using Pork.Shared;
using Pork.Shared.Entities;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Responses;
using Serilog;

Log.Logger = LoggerUtils.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(g => {
    g.UseOneOfForPolymorphism();
    g.SupportNonNullableReferenceTypes();
    g.UseAllOfForInheritance();
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(
        policy => {
            policy.WithOrigins("http://localhost", "http://localhost:5173")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        });
});

builder.Services.AddDbContext<DataContext>();

var app = builder.Build();


if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();

var clients = app.MapGroup("/clients");
clients.MapGet("/",
        async ([FromServices] DataContext dataContext) =>
            (await dataContext.GlobalClients.ToListAsync()).Select(ClientDto.From))
    .WithName("GetClients")
    .WithTags("clients");


var specificClient = clients.MapGroup("/{clientId}");
specificClient.MapGet("", async ([FromServices] DataContext dataContext, Guid clientId) => {
        var client = await dataContext.GlobalClients.FirstOrDefaultAsync(c => c.Id == clientId);
        return client is null ? Results.NotFound() : Results.Ok(ClientDto.From(client));
    })
    .Produces<ClientDto>()
    .WithName("GetClient")
    .WithTags("clients");

specificClient.MapGet("/logs",
        async ([FromServices] DataContext dataContext, Guid clientId, [FromQuery] [Range(1, 100)] int count,
                [FromQuery] int offset) =>
            (await dataContext.ClientLogs.Where(e => e.LocalClientId == clientId)
                .OrderByDescending(e => e.Timestamp)
                .Skip(offset)
                .Take(count)
                .ToListAsync()).Select(ClientLogDto.From))
    .WithName("GetClientLogs")
    .WithTags("logs");

specificClient.MapGet("/events",
        async ([FromServices] DataContext dataContext, Guid clientId, [FromQuery] int count,
                [FromQuery] int offset) =>
            (await dataContext.ClientMessages.Where(e => e.LocalClientId == clientId)
                .OrderByDescending(e => e.Timestamp)
                .Skip(offset)
                .Take(count)
                .ToListAsync())
            .Select(DtoMapper.MapMessage))
    .WithName("GetClientEvents")
    .WithTags("events");

specificClient.MapGet("/events/console",
        async ([FromServices] DataContext dataContext, Guid clientId, [FromQuery] int count,
            [FromQuery] int offset) => {
            var requests = await dataContext.ClientMessages
                .Where(e => e.LocalClientId == clientId && e.ShowInConsole)
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
    .WithName("GetClientConsoleEvents")
    .WithTags("events");


specificClient.MapPut("/nickname",
        async ([FromServices] DataContext dataContext, Guid clientId, [FromBody] SetNicknameRequestDto request)
            => {
            var client = await dataContext.GlobalClients.FirstOrDefaultAsync(c => c.Id == clientId);
            if (client is null) {
                return Results.NotFound();
            }

            client.Nickname = request.Nickname;
            await dataContext.SaveChangesAsync();
            return Results.Ok();
        })
    .WithName("SetNickname")
    .WithTags("clients");

var clientActions = specificClient.MapGroup("/actions");

clientActions.MapPost("/eval",
        async ([FromServices] DataContext dataContext, Guid clientId, [FromBody] EvalRequestDto request) => {
            var client = await dataContext.GlobalClients.FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null) {
                return Results.NotFound();
            }

            var eval = new ClientEvalRequest {
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
    .WithName("clientEval")
    .WithTags("actions");

app.Run();