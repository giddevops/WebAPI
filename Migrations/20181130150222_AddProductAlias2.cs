using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddProductAlias2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProductAlias_ProductId",
                table: "ProductAlias",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAlias_Product_ProductId",
                table: "ProductAlias",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAlias_Product_ProductId",
                table: "ProductAlias");

            migrationBuilder.DropIndex(
                name: "IX_ProductAlias_ProductId",
                table: "ProductAlias");
        }
    }
}
