using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class go34dweffwefjweh2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerEvent_ScannerEventId",
                table: "ScannerEventScannerLabelType");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerEvent_ScannerEventId",
                table: "ScannerEventScannerLabelType",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerEvent_ScannerEventId",
                table: "ScannerEventScannerLabelType");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerEvent_ScannerEventId",
                table: "ScannerEventScannerLabelType",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
