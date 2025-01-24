using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class WorrrrK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_IncomingShipment_ShippingCarrierId",
                table: "IncomingShipment",
                column: "ShippingCarrierId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingShipment_ShippingCarrier_ShippingCarrierId",
                table: "IncomingShipment",
                column: "ShippingCarrierId",
                principalTable: "ShippingCarrier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingShipment_ShippingCarrier_ShippingCarrierId",
                table: "IncomingShipment");

            migrationBuilder.DropIndex(
                name: "IX_IncomingShipment_ShippingCarrierId",
                table: "IncomingShipment");
        }
    }
}
