using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class ADdScannerLabels3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EndScannerLabel_ScannerLabel_StartScannerLabelId",
                table: "EndScannerLabel");

            migrationBuilder.AddForeignKey(
                name: "FK_EndScannerLabel_ScannerLabel_StartScannerLabelId",
                table: "EndScannerLabel",
                column: "StartScannerLabelId",
                principalTable: "ScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EndScannerLabel_ScannerLabel_StartScannerLabelId",
                table: "EndScannerLabel");

            migrationBuilder.AddForeignKey(
                name: "FK_EndScannerLabel_ScannerLabel_StartScannerLabelId",
                table: "EndScannerLabel",
                column: "StartScannerLabelId",
                principalTable: "ScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
