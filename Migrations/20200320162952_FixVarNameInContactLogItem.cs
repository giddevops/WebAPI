using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class FixVarNameInContactLogItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactReasonId",
                table: "ContactLogItem",
                newName: "ContactReasonOptionId");

            migrationBuilder.RenameColumn(
                name: "ContactMethodId",
                table: "ContactLogItem",
                newName: "ContactMethodOptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactReasonOptionId",
                table: "ContactLogItem",
                newName: "ContactReasonId");

            migrationBuilder.RenameColumn(
                name: "ContactMethodOptionId",
                table: "ContactLogItem",
                newName: "ContactMethodId");
        }
    }
}
