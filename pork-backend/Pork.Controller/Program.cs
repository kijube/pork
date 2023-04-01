using System.Net.WebSockets;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.FileProviders;
using Pork.Controller;
using Pork.Shared;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = LoggerUtils.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts => { opts.SupportNonNullableReferenceTypes(); });
builder.Services.AddWebSockets(opts => { });
builder.Services.AddScoped<DataContext>();
builder.Services.AddScoped<ClientConnector>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Join(app.Environment.ContentRootPath, "static"))
});

app.UseWebSockets();
app.Use(async (context, next) => {
    if (context.Request.Path != "/connect") {
        await next(context);
        return;
    }

    if (!context.WebSockets.IsWebSocketRequest) {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    var connector = context.RequestServices.GetRequiredService<ClientConnector>();
    var tcs = new TaskCompletionSource();
    await Task.Factory.StartNew(async () => {
        await connector.ConnectAsync(context);
        tcs.SetResult();
    }, TaskCreationOptions.LongRunning);
    await tcs.Task;
});

app.Run();