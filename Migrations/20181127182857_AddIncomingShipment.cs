using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddIncomingShipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentLocation",
                table: "IncomingShipment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentShippingStatus",
                table: "IncomingShipment",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateShipped",
                table: "IncomingShipment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiptSignerId",
                table: "IncomingShipment",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShipmentTrackingEvent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Order = table.Column<int>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ExceptionCode = table.Column<string>(nullable: true),
                    ExceptionDescription = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    CountryCode = table.Column<string>(nullable: true),
                    CountryName = table.Column<string>(nullable: true),
                    OrganizationName = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    StateProvinceCode = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentTrackingEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncomingShipmentShipmentTrackingEvent",
                columns: table => new
                {
                    ShipmentTrackingEventId = table.Column<int>(nullable: false),
                    IncomingShipmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingShipmentShipmentTrackingEvent", x => new { x.IncomingShipmentId, x.ShipmentTrackingEventId });
                    table.ForeignKey(
                        name: "FK_IncomingShipmentShipmentTrackingEvent_IncomingShipment_IncomingShipmentId",
                        column: x => x.IncomingShipmentId,
                        principalTable: "IncomingShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomingShipmentShipmentTrackingEvent_ShipmentTrackingEvent_IncomingShipmentId",
                        column: x => x.IncomingShipmentId,
                        principalTable: "ShipmentTrackingEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingShipmentShipmentTrackingEvent",
                columns: table => new
                {
                    ShipmentTrackingEventId = table.Column<int>(nullable: false),
                    OutgoingShipmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingShipmentShipmentTrackingEvent", x => new { x.OutgoingShipmentId, x.ShipmentTrackingEventId });
                    table.ForeignKey(
                        name: "FK_OutgoingShipmentShipmentTrackingEvent_OutgoingShipment_OutgoingShipmentId",
                        column: x => x.OutgoingShipmentId,
                        principalTable: "OutgoingShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingShipmentShipmentTrackingEvent_ShipmentTrackingEvent_OutgoingShipmentId",
                        column: x => x.OutgoingShipmentId,
                        principalTable: "ShipmentTrackingEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingShipment_ReceiptSignerId",
                table: "IncomingShipment",
                column: "ReceiptSignerId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingShipment_User_ReceiptSignerId",
                table: "IncomingShipment",
                column: "ReceiptSignerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingShipment_User_ReceiptSignerId",
                table: "IncomingShipment");

            migrationBuilder.DropTable(
                name: "IncomingShipmentShipmentTrackingEvent");

            migrationBuilder.DropTable(
                name: "OutgoingShipmentShipmentTrackingEvent");

            migrationBuilder.DropTable(
                name: "ShipmentTrackingEvent");

            migrationBuilder.DropIndex(
                name: "IX_IncomingShipment_ReceiptSignerId",
                table: "IncomingShipment");

            migrationBuilder.DropColumn(
                name: "CurrentLocation",
                table: "IncomingShipment");

            migrationBuilder.DropColumn(
                name: "CurrentShippingStatus",
                table: "IncomingShipment");

            migrationBuilder.DropColumn(
                name: "DateShipped",
                table: "IncomingShipment");

            migrationBuilder.DropColumn(
                name: "ReceiptSignerId",
                table: "IncomingShipment");
        }
    }
}
