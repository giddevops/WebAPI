using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddEndScannerLabelToScan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scan_ScannerLabel_ScannerLabelId",
                table: "Scan");

            migrationBuilder.AlterColumn<int>(
                name: "ScannerLabelId",
                table: "Scan",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "EndScannerLabelId",
                table: "Scan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scan_EndScannerLabelId",
                table: "Scan",
                column: "EndScannerLabelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scan_ScannerLabel_EndScannerLabelId",
                table: "Scan",
                column: "EndScannerLabelId",
                principalTable: "ScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Scan_ScannerLabel_ScannerLabelId",
                table: "Scan",
                column: "ScannerLabelId",
                principalTable: "ScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scan_ScannerLabel_EndScannerLabelId",
                table: "Scan");

            migrationBuilder.DropForeignKey(
                name: "FK_Scan_ScannerLabel_ScannerLabelId",
                table: "Scan");

            migrationBuilder.DropIndex(
                name: "IX_Scan_EndScannerLabelId",
                table: "Scan");

            migrationBuilder.DropColumn(
                name: "EndScannerLabelId",
                table: "Scan");

            migrationBuilder.AlterColumn<int>(
                name: "ScannerLabelId",
                table: "Scan",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Scan_ScannerLabel_ScannerLabelId",
                table: "Scan",
                column: "ScannerLabelId",
                principalTable: "ScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
