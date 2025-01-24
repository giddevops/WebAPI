using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddValueScannerLabelTypeVariableIdToScannerActionUpdateSystemDataCommand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ValueScannerLabelTypeVariableId",
                table: "ScannerActionUpdateSystemDataCommand",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueScannerLabelTypeVariableId",
                table: "ScannerActionUpdateSystemDataCommand");
        }
    }
}
