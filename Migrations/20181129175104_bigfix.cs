using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class bigfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingShipmentShipmentTrackingEvent_ShipmentTrackingEvent_IncomingShipmentId",
                table: "IncomingShipmentShipmentTrackingEvent");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingShipmentShipmentTrackingEvent_ShipmentTrackingEventId",
                table: "IncomingShipmentShipmentTrackingEvent",
                column: "ShipmentTrackingEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingShipmentShipmentTrackingEvent_ShipmentTrackingEvent_ShipmentTrackingEventId",
                table: "IncomingShipmentShipmentTrackingEvent",
                column: "ShipmentTrackingEventId",
                principalTable: "ShipmentTrackingEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingShipmentShipmentTrackingEvent_ShipmentTrackingEvent_ShipmentTrackingEventId",
                table: "IncomingShipmentShipmentTrackingEvent");

            migrationBuilder.DropIndex(
                name: "IX_IncomingShipmentShipmentTrackingEvent_ShipmentTrackingEventId",
                table: "IncomingShipmentShipmentTrackingEvent");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingShipmentShipmentTrackingEvent_ShipmentTrackingEvent_IncomingShipmentId",
                table: "IncomingShipmentShipmentTrackingEvent",
                column: "IncomingShipmentId",
                principalTable: "ShipmentTrackingEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
