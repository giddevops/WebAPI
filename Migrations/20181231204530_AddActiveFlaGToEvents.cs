using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddActiveFlaGToEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerEvent_ScannerEventId",
                table: "ScannerEventScannerLabelType");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerEventScannerLabelType");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ScannerActionUpdateWorkLog",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ScannerActionUpdateSystemData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ScannerActionUpdateLocation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ScannerActionRelatePieceParts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerEvent_ScannerEventId",
                table: "ScannerEventScannerLabelType",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerEventScannerLabelType",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerEvent_ScannerEventId",
                table: "ScannerEventScannerLabelType");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerEventScannerLabelType");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "ScannerActionUpdateLocation");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerEvent_ScannerEventId",
                table: "ScannerEventScannerLabelType",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerEventScannerLabelType_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerEventScannerLabelType",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
