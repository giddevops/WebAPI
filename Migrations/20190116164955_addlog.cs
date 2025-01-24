using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class addlog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessLogItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLogItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogItem_CreatedAt",
                table: "AccessLogItem",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogItem_UserId_CreatedAt",
                table: "AccessLogItem",
                columns: new[] { "UserId", "CreatedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessLogItem");
        }
    }
}
