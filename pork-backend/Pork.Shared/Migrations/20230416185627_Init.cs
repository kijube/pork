using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pork.Shared.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ClientMessageSequence");

            migrationBuilder.CreateTable(
                name: "GlobalClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RemoteIp = table.Column<string>(type: "text", nullable: true),
                    Nickname = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GlobalClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    SiteId = table.Column<int>(type: "integer", nullable: false),
                    LastSeen = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalClients_GlobalClients_GlobalClientId",
                        column: x => x.GlobalClientId,
                        principalTable: "GlobalClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocalClients_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientEvalResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"ClientMessageSequence\"')"),
                    FlowId = table.Column<Guid>(type: "uuid", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LocalClientId = table.Column<int>(type: "integer", nullable: false),
                    ShowInConsole = table.Column<bool>(type: "boolean", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEvalResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientEvalResponses_LocalClients_LocalClientId",
                        column: x => x.LocalClientId,
                        principalTable: "LocalClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientFailureResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"ClientMessageSequence\"')"),
                    FlowId = table.Column<Guid>(type: "uuid", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LocalClientId = table.Column<int>(type: "integer", nullable: false),
                    ShowInConsole = table.Column<bool>(type: "boolean", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientFailureResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientFailureResponses_LocalClients_LocalClientId",
                        column: x => x.LocalClientId,
                        principalTable: "LocalClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientHookResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"ClientMessageSequence\"')"),
                    FlowId = table.Column<Guid>(type: "uuid", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LocalClientId = table.Column<int>(type: "integer", nullable: false),
                    ShowInConsole = table.Column<bool>(type: "boolean", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: false),
                    HookId = table.Column<string>(type: "text", nullable: false),
                    Args = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientHookResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientHookResponses_LocalClients_LocalClientId",
                        column: x => x.LocalClientId,
                        principalTable: "LocalClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocalClientId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientLogs_LocalClients_LocalClientId",
                        column: x => x.LocalClientId,
                        principalTable: "LocalClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientEvalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"ClientMessageSequence\"')"),
                    FlowId = table.Column<Guid>(type: "uuid", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LocalClientId = table.Column<int>(type: "integer", nullable: false),
                    ShowInConsole = table.Column<bool>(type: "boolean", nullable: false),
                    Sent = table.Column<bool>(type: "boolean", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: false),
                    ResponseId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEvalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientEvalRequests_ClientEvalResponses_ResponseId",
                        column: x => x.ResponseId,
                        principalTable: "ClientEvalResponses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientEvalRequests_LocalClients_LocalClientId",
                        column: x => x.LocalClientId,
                        principalTable: "LocalClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientEvalRequests_LocalClientId",
                table: "ClientEvalRequests",
                column: "LocalClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEvalRequests_ResponseId",
                table: "ClientEvalRequests",
                column: "ResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEvalResponses_LocalClientId",
                table: "ClientEvalResponses",
                column: "LocalClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientFailureResponses_LocalClientId",
                table: "ClientFailureResponses",
                column: "LocalClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientHookResponses_LocalClientId",
                table: "ClientHookResponses",
                column: "LocalClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLogs_LocalClientId",
                table: "ClientLogs",
                column: "LocalClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalClients_RemoteIp",
                table: "GlobalClients",
                column: "RemoteIp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalClients_GlobalClientId_SiteId",
                table: "LocalClients",
                columns: new[] { "GlobalClientId", "SiteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalClients_SiteId",
                table: "LocalClients",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Key",
                table: "Sites",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientEvalRequests");

            migrationBuilder.DropTable(
                name: "ClientFailureResponses");

            migrationBuilder.DropTable(
                name: "ClientHookResponses");

            migrationBuilder.DropTable(
                name: "ClientLogs");

            migrationBuilder.DropTable(
                name: "ClientEvalResponses");

            migrationBuilder.DropTable(
                name: "LocalClients");

            migrationBuilder.DropTable(
                name: "GlobalClients");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropSequence(
                name: "ClientMessageSequence");
        }
    }
}
