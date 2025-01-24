using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddScannerStationScannerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScannerId",
                table: "ScannerStation",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScannerStation_ScannerId",
                table: "ScannerStation",
                column: "ScannerId",
                unique: true,
                filter: "[ScannerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerStation_Scanner_ScannerId",
                table: "ScannerStation",
                column: "ScannerId",
                principalTable: "Scanner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerStation_Scanner_ScannerId",
                table: "ScannerStation");

            migrationBuilder.DropIndex(
                name: "IX_ScannerStation_ScannerId",
                table: "ScannerStation");

            migrationBuilder.DropColumn(
                name: "ScannerId",
                table: "ScannerStation");
        }
    }
}
