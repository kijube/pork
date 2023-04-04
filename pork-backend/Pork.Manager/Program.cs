using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Pork.Manager;
using Pork.Manager.Dtos;
using Pork.Manager.Dtos.Messages;
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
            policy.WithOrigins("http://localhost")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        });
});

builder.Services.AddScoped<DataContext>();

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
            (await dataContext.Clients.Find(_ => true).ToListAsync()).Select(ClientDto.From))
    .WithName("GetClients")
    .WithTags("clients");


var specificClient = clients.MapGroup("/{clientId}");
specificClient.MapGet("", async ([FromServices] DataContext dataContext, string clientId) =>
        ClientDto.From(await dataContext.Clients.Find(c => c.ClientId == clientId).FirstOrDefaultAsync()))
    .WithName("GetClient")
    .WithTags("clients");

specificClient.MapGet("/logs",
        async ([FromServices] DataContext dataContext, string clientId, [FromQuery] [Range(1, 100)] int count,
                [FromQuery] int offset) =>
            (await dataContext.ClientLogs.Find(e => e.ClientId == clientId)
                .SortByDescending(e => e.Timestamp)
                .Skip(offset)
                .Limit(count)
                .ToListAsync()).Select(ClientLogDto.From))
    .WithName("GetClientLogs")
    .WithTags("logs");

specificClient.MapGet("/events",
        async ([FromServices] DataContext dataContext, string clientId, [FromQuery] int count,
                [FromQuery] int offset) =>
            (await dataContext.ClientMessages.Find(e => e.ClientId == clientId)
                .SortByDescending(e => e.Timestamp)
                .Skip(offset)
                .Limit(count)
                .ToListAsync())
            .Select(DtoMapper.MapMessage))
    .WithName("GetClientEvents")
    .WithTags("events");

specificClient.MapGet("/events/eval",
    async ([FromServices] DataContext dataContext, string clientId, [FromQuery] int count, [FromQuery] int offset) => {
        var requests = await dataContext.ClientMessages.OfType<ClientEvalRequest>()
            .Find(e => e.ClientId == clientId)
            .SortByDescending(e => e.Timestamp)
            .Skip(offset)
            .Limit(count)
            .ToListAsync();

        var requestFlowIds = requests.Select(r => r.FlowId).Where(f => f is not null).ToList();

        var responses = await dataContext.ClientMessages.OfType<ClientEvalResponse>()
            .Find(r => requestFlowIds.Contains(r.FlowId))
            .ToListAsync();

        return requests.GroupJoin(responses, r => r.FlowId, r => r.FlowId, (request, response) => {
            var responseList = response.ToList();
            return InternalEvalFlow.From(request, responseList.Count > 0 ? responseList[0] : null);
        });
    });


specificClient.MapPut("/nickname",
        async ([FromServices] DataContext dataContext, string clientId, [FromBody] SetNicknameRequestDto request)
            => {
            var result = await dataContext.Clients.UpdateOneAsync(c => c.ClientId == clientId,
                Builders<Client>.Update.Set(c => c.Nickname, request.Nickname));
            return result?.IsAcknowledged ?? false;
        })
    .WithName("SetNickname")
    .WithTags("clients");

var clientActions = specificClient.MapGroup("/actions");

clientActions.MapPost("/eval",
        async ([FromServices] DataContext dataContext, string clientId, [FromBody] EvalRequestDto request) => {
            var client = await dataContext.Clients.Find(c => c.ClientId == clientId).FirstOrDefaultAsync();
            if (client == null) {
                return Results.NotFound();
            }

            var eval = new ClientEvalRequest
            {
                Timestamp = DateTimeOffset.Now,
                ClientId = client.ClientId,
                Code = request.Code,
                FlowId = Guid.NewGuid(),
            };

            await dataContext.ClientMessages.InsertOneAsync(eval);
            return Results.Ok(DtoMapper.MapRequest(eval) as InternalEvalRequest);
        })
    .Produces<InternalEvalRequest>()
    .Produces(StatusCodes.Status404NotFound)
    .WithName("clientEval")
    .WithTags("actions");

app.Run();