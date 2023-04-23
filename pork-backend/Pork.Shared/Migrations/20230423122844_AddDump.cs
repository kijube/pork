using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pork.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddDump : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {


            migrationBuilder.CreateTable(
                name: "ClientDumpResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false,
                        defaultValueSql: "nextval('\"ClientMessageSequence\"')"),
                    FlowId = table.Column<Guid>(type: "uuid", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LocalClientId = table.Column<int>(type: "integer", nullable: false),
                    ShowInConsole = table.Column<bool>(type: "boolean", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Dump = table.Column<string>(type: "json", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ClientDumpResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientDumpResponses_LocalClients_LocalClientId",
                        column: x => x.LocalClientId,
                        principalTable: "LocalClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientDumpResponses_LocalClientId",
                table: "ClientDumpResponses",
                column: "LocalClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "ClientDumpResponses");
        }
    }
}
