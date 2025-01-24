using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddPartNumberNoSpecialCharsIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PartNumberNoSpecialChars",
                table: "ProductAlias",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartNumberNoSpecialChars",
                table: "Product",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAlias_PartNumberNoSpecialChars",
                table: "ProductAlias",
                column: "PartNumberNoSpecialChars");

            migrationBuilder.CreateIndex(
                name: "IX_Product_PartNumberNoSpecialChars",
                table: "Product",
                column: "PartNumberNoSpecialChars");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductAlias_PartNumberNoSpecialChars",
                table: "ProductAlias");

            migrationBuilder.DropIndex(
                name: "IX_Product_PartNumberNoSpecialChars",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "PartNumberNoSpecialChars",
                table: "ProductAlias",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartNumberNoSpecialChars",
                table: "Product",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
