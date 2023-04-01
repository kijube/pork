using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Pork.Manager;
using Pork.Shared;
using Pork.Shared.Entities;
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

builder.Services.AddScoped<DataContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

var clients = app.MapGroup("/clients");
clients.MapGet("/",
        async ([FromServices] DataContext dataContext) => await dataContext.Clients.Find(_ => true).ToListAsync())
    .WithName("GetClients")
    .WithTags("clients");


var specificClient = clients.MapGroup("/{clientId}");
specificClient.MapGet("", async ([FromServices] DataContext dataContext, string clientId) =>
        await dataContext.Clients.Find(c => c.Id == clientId).FirstOrDefaultAsync())
    .WithName("GetClient")
    .WithTags("clients");

specificClient.MapGet("/logs",
        async ([FromServices] DataContext dataContext, string clientId, [FromQuery] [Range(1, 100)] int count,
                [FromQuery] int offset) =>
            await dataContext.ClientLogs.Find(e => e.ClientId == clientId)
                .SortByDescending(e => e.Timestamp)
                .Skip(offset)
                .Limit(count)
                .ToListAsync())
    .WithName("GetClientLogs")
    .WithTags("logs");

specificClient.MapGet("/events",
        async ([FromServices] DataContext dataContext, string clientId, [FromQuery] int count,
                [FromQuery] int offset) =>
            await dataContext.ClientResponses.Find(e => e.ClientId == clientId)
                .SortByDescending(e => e.Timestamp)
                .Skip(offset)
                .Limit(count)
                .ToListAsync())
    .WithName("GetClientEvents")
    .WithTags("events");

specificClient.MapPut("/nickname",
        async ([FromServices] DataContext dataContext, string clientId, [FromBody] SetNicknameRequest request)
            => {
            var result = await dataContext.Clients.UpdateOneAsync(c => c.Id == clientId,
                Builders<Client>.Update.Set(c => c.Nickname, request.Nickname));
            return result?.IsAcknowledged ?? false;
        })
    .WithName("SetNickname")
    .WithTags("clients");


app.Run();