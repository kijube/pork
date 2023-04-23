﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pork.Shared;

#nullable disable

namespace Pork.Shared.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230423122844_AddDump")]
    partial class AddDump
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.HasSequence("ClientMessageSequence");

            modelBuilder.Entity("Pork.Shared.Entities.ClientLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("LocalClientId")
                        .HasColumnType("integer");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("LocalClientId");

                    b.ToTable("ClientLogs");
                });

            modelBuilder.Entity("Pork.Shared.Entities.GlobalClient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Nickname")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("GlobalClients");
                });

            modelBuilder.Entity("Pork.Shared.Entities.LocalClient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("GlobalClientId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("LastSeen")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RemoteIp")
                        .HasColumnType("text");

                    b.Property<int>("SiteId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SiteId");

                    b.HasIndex("GlobalClientId", "SiteId")
                        .IsUnique();

                    b.ToTable("LocalClients");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.ClientMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValueSql("nextval('\"ClientMessageSequence\"')");

                    NpgsqlPropertyBuilderExtensions.UseSequence(b.Property<int>("Id"));

                    b.Property<Guid?>("FlowId")
                        .HasColumnType("uuid");

                    b.Property<int>("LocalClientId")
                        .HasColumnType("integer");

                    b.Property<bool>("ShowInConsole")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("LocalClientId");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Site.SiteMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("FlowId")
                        .HasColumnType("uuid");

                    b.Property<int>("SiteId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("SiteId");

                    b.ToTable("SiteMessages");

                    b.HasDiscriminator<string>("Discriminator").HasValue("SiteMessage");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Pork.Shared.Entities.Site", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Requests.ClientRequest", b =>
                {
                    b.HasBaseType("Pork.Shared.Entities.Messages.ClientMessage");

                    b.Property<bool>("Sent")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("SentAt")
                        .HasColumnType("timestamp with time zone");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Responses.ClientResponse", b =>
                {
                    b.HasBaseType("Pork.Shared.Entities.Messages.ClientMessage");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Site.SiteBroadcastMessage", b =>
                {
                    b.HasBaseType("Pork.Shared.Entities.Messages.Site.SiteMessage");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("SiteBroadcastMessage");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Requests.ClientEvalRequest", b =>
                {
                    b.HasBaseType("Pork.Shared.Entities.Messages.Requests.ClientRequest");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ResponseId")
                        .HasColumnType("integer");

                    b.HasIndex("ResponseId");

                    b.ToTable("ClientEvalRequests");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Responses.ClientDumpResponse", b =>
                {
                    b.HasBaseType("Pork.Shared.Entities.Messages.Responses.ClientResponse");

                    b.Property<string>("Dump")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("ClientDumpResponses");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Responses.ClientEvalResponse", b =>
                {
                    b.HasBaseType("Pork.Shared.Entities.Messages.Responses.ClientResponse");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("ClientEvalResponses");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Responses.ClientFailureResponse", b =>
                {
                    b.HasBaseType("Pork.Shared.Entities.Messages.Responses.ClientResponse");

                    b.Property<string>("Error")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("ClientFailureResponses");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Responses.ClientHookResponse", b =>
                {
                    b.HasBaseType("Pork.Shared.Entities.Messages.Responses.ClientResponse");

                    b.Property<string>("Args")
                        .HasColumnType("text");

                    b.Property<string>("HookId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Method")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Result")
                        .HasColumnType("text");

                    b.ToTable("ClientHookResponses");
                });

            modelBuilder.Entity("Pork.Shared.Entities.ClientLog", b =>
                {
                    b.HasOne("Pork.Shared.Entities.LocalClient", "LocalClient")
                        .WithMany()
                        .HasForeignKey("LocalClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LocalClient");
                });

            modelBuilder.Entity("Pork.Shared.Entities.LocalClient", b =>
                {
                    b.HasOne("Pork.Shared.Entities.GlobalClient", "GlobalClient")
                        .WithMany()
                        .HasForeignKey("GlobalClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pork.Shared.Entities.Site", "Site")
                        .WithMany("LocalClients")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GlobalClient");

                    b.Navigation("Site");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.ClientMessage", b =>
                {
                    b.HasOne("Pork.Shared.Entities.LocalClient", "LocalClient")
                        .WithMany()
                        .HasForeignKey("LocalClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LocalClient");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Site.SiteMessage", b =>
                {
                    b.HasOne("Pork.Shared.Entities.Site", "Site")
                        .WithMany()
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Site");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Messages.Requests.ClientEvalRequest", b =>
                {
                    b.HasOne("Pork.Shared.Entities.Messages.Responses.ClientEvalResponse", "Response")
                        .WithMany()
                        .HasForeignKey("ResponseId");

                    b.Navigation("Response");
                });

            modelBuilder.Entity("Pork.Shared.Entities.Site", b =>
                {
                    b.Navigation("LocalClients");
                });
#pragma warning restore 612, 618
        }
    }
}