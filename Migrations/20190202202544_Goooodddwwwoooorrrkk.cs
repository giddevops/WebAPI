using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class Goooodddwwwoooorrrkk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerLabelType_InventoryItemScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionUpdateWorkLog_InventoryItemScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropColumn(
                name: "InventoryItemScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InventoryItemScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateWorkLog_InventoryItemScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                column: "InventoryItemScannerLabelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerLabelType_InventoryItemScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                column: "InventoryItemScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
