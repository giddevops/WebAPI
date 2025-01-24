using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddSerialNumberAndModelNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ModelNumber",
                table: "Scanner",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scanner_SerialNumber_ModelNumber",
                table: "Scanner",
                columns: new[] { "SerialNumber", "ModelNumber" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Scanner_SerialNumber_ModelNumber",
                table: "Scanner");

            migrationBuilder.AlterColumn<string>(
                name: "ModelNumber",
                table: "Scanner",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
