﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pork.Shared.Entities;
using Pork.Shared.Entities.Messages;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Shared;

public class DataContext : DbContext {
    public DbSet<GlobalClient> GlobalClients { get; init; }
    public DbSet<LocalClient> LocalClients { get; init; }
    public DbSet<Site> Sites { get; init; }
    public DbSet<ClientLog> ClientLogs { get; init; }

    public DbSet<ClientMessage> ClientMessages { get; init; }
    public DbSet<ClientRequest> ClientRequests { get; init; }
    public DbSet<ClientEvalRequest> ClientEvalRequests { get; init; }

    public DbSet<ClientResponse> ClientResponses { get; set; }
    public DbSet<ClientFailureResponse> ClientFailureResponses { get; set; }
    public DbSet<ClientEvalResponse> ClientEvalResponses { get; set; }
    public DbSet<ClientHookResponse> ClientHookResponses { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(
            "User ID=postgres;Password=pork;Host=localhost;Port=5432;Database=pork;Include Error Detail=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ClientMessage>().UseTpcMappingStrategy();

        modelBuilder.Entity<LocalClient>().HasIndex(c => new {c.GlobalClientId, c.SiteId}).IsUnique();
        modelBuilder.Entity<GlobalClient>().HasIndex(c => new {c.RemoteIp}).IsUnique();
        modelBuilder.Entity<Site>().HasIndex(c => new {c.Key}).IsUnique();
    }
}