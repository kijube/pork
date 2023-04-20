using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pork.Shared.Migrations
{
    /// <inheritdoc />
    public partial class MoveRemoteIp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "RemoteIp",
                table: "GlobalClients");

            migrationBuilder.AddColumn<string>(
                name: "RemoteIp",
                table: "LocalClients",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "RemoteIp",
                table: "LocalClients");

            migrationBuilder.AddColumn<string>(
                name: "RemoteIp",
                table: "GlobalClients",
                type: "text",
                nullable: true);
        }
    }
}
