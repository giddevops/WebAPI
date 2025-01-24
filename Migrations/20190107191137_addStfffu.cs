using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class addStfffu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Scan_Scanner_ScannerLabelId",
            //     table: "Scan");

            migrationBuilder.AddColumn<int>(
                name: "ScannerId",
                table: "Scan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scan_ScannerId",
                table: "Scan",
                column: "ScannerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scan_Scanner_ScannerId",
                table: "Scan",
                column: "ScannerId",
                principalTable: "Scanner",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scan_Scanner_ScannerId",
                table: "Scan");

            migrationBuilder.DropIndex(
                name: "IX_Scan_ScannerId",
                table: "Scan");

            migrationBuilder.DropColumn(
                name: "ScannerId",
                table: "Scan");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Scan_Scanner_ScannerLabelId",
            //     table: "Scan",
            //     column: "ScannerLabelId",
            //     principalTable: "Scanner",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.SetNull);
        }
    }
}
