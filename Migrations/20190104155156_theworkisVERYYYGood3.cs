using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class theworkisVERYYYGood3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateLocation_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateLocation");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateLocation_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateLocation",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateLocation_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateLocation");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateLocation_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateLocation",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
