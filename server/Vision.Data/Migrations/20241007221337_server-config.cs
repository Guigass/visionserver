using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Data.Migrations
{
    /// <inheritdoc />
    public partial class serverconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OnlyProcessWhenIsRequested = table.Column<bool>(type: "boolean", nullable: false),
                    IdleTimeToStopProcess = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerConfigs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerConfigs");
        }
    }
}
