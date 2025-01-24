using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class stuffff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ScannerActionUpdateSystemDataCommand",
                nullable: true,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateSystemDataCommand_ValueScannerLabelTypeVariableId",
                table: "ScannerActionUpdateSystemDataCommand",
                column: "ValueScannerLabelTypeVariableId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateSystemDataCommand_ScannerLabelTypeVariable_ValueScannerLabelTypeVariableId",
                table: "ScannerActionUpdateSystemDataCommand",
                column: "ValueScannerLabelTypeVariableId",
                principalTable: "ScannerLabelTypeVariable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateSystemDataCommand_ScannerLabelTypeVariable_ValueScannerLabelTypeVariableId",
                table: "ScannerActionUpdateSystemDataCommand");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionUpdateSystemDataCommand_ValueScannerLabelTypeVariableId",
                table: "ScannerActionUpdateSystemDataCommand");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ScannerActionUpdateSystemDataCommand",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true,
                oldDefaultValueSql: "GETUTCDATE()");
        }
    }
}
