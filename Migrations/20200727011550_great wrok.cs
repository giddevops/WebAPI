using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class greatwrok : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoInserted",
                table: "Contact",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AutoInserted",
                table: "Company",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoInserted",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "AutoInserted",
                table: "Company");
        }
    }
}
