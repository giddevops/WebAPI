using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class adddthenewendscannerllabell3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "EndScannerLabel",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime));

            migrationBuilder.CreateIndex(
                name: "IX_EndScannerLabel_BarcodeGuid",
                table: "EndScannerLabel",
                column: "BarcodeGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EndScannerLabel_BarcodeGuid",
                table: "EndScannerLabel");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "EndScannerLabel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "GETUTCDATE()");
        }
    }
}
