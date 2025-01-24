using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddScannerStationToScan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScannerStationId",
                table: "Scan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scan_ScannerStationId",
                table: "Scan",
                column: "ScannerStationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Scan_ScannerStationId",
                table: "Scan");

            migrationBuilder.DropColumn(
                name: "ScannerStationId",
                table: "Scan");
        }
    }
}
