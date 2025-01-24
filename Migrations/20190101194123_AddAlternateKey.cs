using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddAlternateKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerEvent_ScannerLabelType_ScannerEventLabelTypeId",
                table: "ScannerEvent");

            migrationBuilder.DropIndex(
                name: "IX_ScannerEvent_ScannerEventLabelTypeId",
                table: "ScannerEvent");

            migrationBuilder.AlterColumn<int>(
                name: "ScannerEventLabelTypeId",
                table: "ScannerEvent",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Open",
                table: "ScanGroup",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ScannerEvent_ScannerEventLabelTypeId",
                table: "ScannerEvent",
                column: "ScannerEventLabelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerEvent_ScannerLabelType_ScannerEventLabelTypeId",
                table: "ScannerEvent",
                column: "ScannerEventLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerEvent_ScannerLabelType_ScannerEventLabelTypeId",
                table: "ScannerEvent");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ScannerEvent_ScannerEventLabelTypeId",
                table: "ScannerEvent");

            migrationBuilder.DropColumn(
                name: "Open",
                table: "ScanGroup");

            migrationBuilder.AlterColumn<int>(
                name: "ScannerEventLabelTypeId",
                table: "ScannerEvent",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_ScannerEvent_ScannerEventLabelTypeId",
                table: "ScannerEvent",
                column: "ScannerEventLabelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerEvent_ScannerLabelType_ScannerEventLabelTypeId",
                table: "ScannerEvent",
                column: "ScannerEventLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
