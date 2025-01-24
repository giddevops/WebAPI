using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class fwlekefwklj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtCST",
                table: "AccessLogItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "AccessLogItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtCST",
                table: "AccessLogItem");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "AccessLogItem");
        }
    }
}
