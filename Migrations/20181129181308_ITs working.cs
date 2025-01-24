using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class ITsworking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingShipmentShipmentTrackingEvent_IncomingShipment_IncomingShipmentId",
                table: "IncomingShipmentShipmentTrackingEvent");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingShipmentShipmentTrackingEvent_IncomingShipment_IncomingShipmentId",
                table: "IncomingShipmentShipmentTrackingEvent",
                column: "IncomingShipmentId",
                principalTable: "IncomingShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingShipmentShipmentTrackingEvent_IncomingShipment_IncomingShipmentId",
                table: "IncomingShipmentShipmentTrackingEvent");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingShipmentShipmentTrackingEvent_IncomingShipment_IncomingShipmentId",
                table: "IncomingShipmentShipmentTrackingEvent",
                column: "IncomingShipmentId",
                principalTable: "IncomingShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
