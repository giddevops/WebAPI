using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class ADdScannerLabelTypeIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ScannerLabelType",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScannerLabelType_ScannerLabelTypeClass_Name",
                table: "ScannerLabelType",
                columns: new[] { "ScannerLabelTypeClass", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScannerLabelType_ScannerLabelTypeClass_Name",
                table: "ScannerLabelType");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ScannerLabelType",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
