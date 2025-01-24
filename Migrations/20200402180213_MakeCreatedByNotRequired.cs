using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class MakeCreatedByNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabel");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedById",
                table: "ScannerLabelVariableValue",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ScannerLabelTypeId",
                table: "ScannerLabel",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedById",
                table: "ScannerLabel",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabel",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabel");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedById",
                table: "ScannerLabelVariableValue",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ScannerLabelTypeId",
                table: "ScannerLabel",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CreatedById",
                table: "ScannerLabel",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabel",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
