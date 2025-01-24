using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class go34dfjweh2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabelVariableValue_ScannerLabel_ScannerLabelId",
                table: "ScannerLabelVariableValue");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ScannerLabel",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "ScannerLabelTypeId1",
                table: "ScannerLabel",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScannerLabel_ScannerLabelTypeId1",
                table: "ScannerLabel",
                column: "ScannerLabelTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId1",
                table: "ScannerLabel",
                column: "ScannerLabelTypeId1",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabelVariableValue_ScannerLabel_ScannerLabelId",
                table: "ScannerLabelVariableValue",
                column: "ScannerLabelId",
                principalTable: "ScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId1",
                table: "ScannerLabel");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabelVariableValue_ScannerLabel_ScannerLabelId",
                table: "ScannerLabelVariableValue");

            migrationBuilder.DropIndex(
                name: "IX_ScannerLabel_ScannerLabelTypeId1",
                table: "ScannerLabel");

            migrationBuilder.DropColumn(
                name: "ScannerLabelTypeId1",
                table: "ScannerLabel");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ScannerLabel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabelVariableValue_ScannerLabel_ScannerLabelId",
                table: "ScannerLabelVariableValue",
                column: "ScannerLabelId",
                principalTable: "ScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
