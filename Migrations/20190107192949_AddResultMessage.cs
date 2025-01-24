using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddResultMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScannerLabelLogEntry");

            migrationBuilder.AddColumn<string>(
                name: "ResultCode",
                table: "Scan",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultMessage",
                table: "Scan",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultCode",
                table: "Scan");

            migrationBuilder.DropColumn(
                name: "ResultMessage",
                table: "Scan");

            migrationBuilder.CreateTable(
                name: "ScannerLabelLogEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ScannerId = table.Column<int>(nullable: true),
                    ScannerStationId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerLabelLogEntry", x => x.Id);
                });
        }
    }
}
