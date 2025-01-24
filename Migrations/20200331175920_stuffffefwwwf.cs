using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class stuffffefwwwf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "BarcodeGuid",
                table: "ScannerLabel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Lead",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Lead");

            migrationBuilder.AlterColumn<Guid>(
                name: "BarcodeGuid",
                table: "ScannerLabel",
                nullable: true,
                oldClrType: typeof(Guid));
        }
    }
}
