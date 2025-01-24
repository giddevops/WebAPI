using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class UpdateStuff5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScannerType",
                table: "ScannerLabelType");

            migrationBuilder.AddColumn<string>(
                name: "ScannerLabelTypeClass",
                table: "ScannerLabelType",
                type: "nvarchar(24)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScannerLabelTypeClass",
                table: "ScannerLabelType");

            migrationBuilder.AddColumn<string>(
                name: "ScannerType",
                table: "ScannerLabelType",
                nullable: true);
        }
    }
}
