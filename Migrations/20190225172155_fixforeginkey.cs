using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class fixforeginkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_Scan_ScannerLabel_EndScannerLabelId",
                table: "Scan");
            migrationBuilder.AddForeignKey(
                name: "FK_Scan_ScannerLabel_EndScannerLabelId",
                table: "Scan",
                column: "EndScannerLabelId",
                principalTable: "EndScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
