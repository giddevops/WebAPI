using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class MoourStoufw : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabelVariableValue_ScannerLabel_ScannerLabelId",
                table: "ScannerLabelVariableValue");

            migrationBuilder.AlterColumn<int>(
                name: "ScannerLabelId",
                table: "ScannerLabelVariableValue",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabelVariableValue_ScannerLabel_ScannerLabelId",
                table: "ScannerLabelVariableValue",
                column: "ScannerLabelId",
                principalTable: "ScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabelVariableValue_ScannerLabel_ScannerLabelId",
                table: "ScannerLabelVariableValue");

            migrationBuilder.AlterColumn<int>(
                name: "ScannerLabelId",
                table: "ScannerLabelVariableValue",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabelVariableValue_ScannerLabel_ScannerLabelId",
                table: "ScannerLabelVariableValue",
                column: "ScannerLabelId",
                principalTable: "ScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
