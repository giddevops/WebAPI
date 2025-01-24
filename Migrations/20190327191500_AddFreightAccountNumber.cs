using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddFreightAccountNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FreightAccountNumber",
                table: "PurchaseOrder",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_ShippingMethodId",
                table: "PurchaseOrder",
                column: "ShippingMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrder_ShippingMethod_ShippingMethodId",
                table: "PurchaseOrder",
                column: "ShippingMethodId",
                principalTable: "ShippingMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_ShippingMethod_ShippingMethodId",
                table: "PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrder_ShippingMethodId",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "FreightAccountNumber",
                table: "PurchaseOrder");
        }
    }
}
