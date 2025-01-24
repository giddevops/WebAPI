using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class ADdScannerLabelType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScannerLabelTypeClass",
                table: "ScannerLabelType"
            );
            migrationBuilder.AddColumn<decimal>(
                name: "ScannerLabelTypeClass",
                table: "ScannerLabelType",
                type: "decimal(16,2)",
                nullable: true);
            // migrationBuilder.AlterColumn<decimal>(
            //     name: "ScannerLabelTypeClass",
            //     table: "ScannerLabelType",
            //     type: "decimal(16,2)",
            //     nullable: false,
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(24)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScannerLabelTypeClass",
                table: "ScannerLabelType"
            );
            migrationBuilder.AddColumn<decimal>(
                name: "ScannerLabelTypeClass",
                table: "ScannerLabelType",
                type: "nvarchar(24)",
                nullable: true);
        }
    }
}
