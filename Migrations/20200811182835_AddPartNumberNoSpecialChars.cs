using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddPartNumberNoSpecialChars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartNumberNoSpecialChars",
                table: "ProductAlias",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartNumberNoSpecialChars",
                table: "Product",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartNumberNoSpecialChars",
                table: "ProductAlias");

            migrationBuilder.DropColumn(
                name: "PartNumberNoSpecialChars",
                table: "Product");
        }
    }
}
