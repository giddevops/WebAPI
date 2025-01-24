using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddProductAliasTypes2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductAliasTypeId",
                table: "ProductAlias",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAlias_ProductAliasTypeId",
                table: "ProductAlias",
                column: "ProductAliasTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAlias_ProductAliasType_ProductAliasTypeId",
                table: "ProductAlias",
                column: "ProductAliasTypeId",
                principalTable: "ProductAliasType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAlias_ProductAliasType_ProductAliasTypeId",
                table: "ProductAlias");

            migrationBuilder.DropIndex(
                name: "IX_ProductAlias_ProductAliasTypeId",
                table: "ProductAlias");

            migrationBuilder.DropColumn(
                name: "ProductAliasTypeId",
                table: "ProductAlias");
        }
    }
}
