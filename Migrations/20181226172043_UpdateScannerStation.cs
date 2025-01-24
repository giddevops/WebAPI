using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class UpdateScannerStation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScannerType",
                table: "ScannerStation");

            migrationBuilder.RenameColumn(
                name: "ScannerLabelTypeId",
                table: "ScannerStation",
                newName: "DefaultScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerStation_DefaultScannerLabelTypeId",
                table: "ScannerStation",
                column: "DefaultScannerLabelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerStation_ScannerLabelType_DefaultScannerLabelTypeId",
                table: "ScannerStation",
                column: "DefaultScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerStation_ScannerLabelType_DefaultScannerLabelTypeId",
                table: "ScannerStation");

            migrationBuilder.DropIndex(
                name: "IX_ScannerStation_DefaultScannerLabelTypeId",
                table: "ScannerStation");

            migrationBuilder.RenameColumn(
                name: "DefaultScannerLabelTypeId",
                table: "ScannerStation",
                newName: "ScannerLabelTypeId");

            migrationBuilder.AddColumn<string>(
                name: "ScannerType",
                table: "ScannerStation",
                nullable: true);
        }
    }
}
