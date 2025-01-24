using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class UpdateFullTextIndexProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Lead",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lead_FullName",
                table: "Lead",
                column: "FullName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lead_FullName",
                table: "Lead");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Lead",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
