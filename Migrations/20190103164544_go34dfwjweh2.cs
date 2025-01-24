using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class go34dfwjweh2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabelTypeVariable_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabelTypeVariable");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabelTypeVariable_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabelTypeVariable",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabelTypeVariable_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabelTypeVariable");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabelTypeVariable_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabelTypeVariable",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
