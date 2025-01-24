using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class stuffwefe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_GidSubLocationOptionId",
                table: "InventoryItem",
                column: "GidSubLocationOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_GidSubLocationOption_GidSubLocationOptionId",
                table: "InventoryItem",
                column: "GidSubLocationOptionId",
                principalTable: "GidSubLocationOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_GidSubLocationOption_GidSubLocationOptionId",
                table: "InventoryItem");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItem_GidSubLocationOptionId",
                table: "InventoryItem");
        }
    }
}
