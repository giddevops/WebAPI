using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class godfjweh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerEvent_ScannerEventId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateSystemData_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropTable(
                name: "ScannerActionUpdateLocation");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerEvent_ScannerEventId",
                table: "ScannerActionRelatePieceParts",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateSystemData_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateSystemData",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateWorkLog",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerEvent_ScannerEventId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateSystemData_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.CreateTable(
                name: "ScannerActionUpdateLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    InventoryItemScannerLabelTypeId = table.Column<int>(nullable: true),
                    ScannerEventId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerActionUpdateLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateLocation_ScannerLabelType_InventoryItemScannerLabelTypeId",
                        column: x => x.InventoryItemScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateLocation_ScannerEvent_ScannerEventId",
                        column: x => x.ScannerEventId,
                        principalTable: "ScannerEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateLocation_InventoryItemScannerLabelTypeId",
                table: "ScannerActionUpdateLocation",
                column: "InventoryItemScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateLocation_ScannerEventId",
                table: "ScannerActionUpdateLocation",
                column: "ScannerEventId",
                unique: true,
                filter: "[ScannerEventId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerEvent_ScannerEventId",
                table: "ScannerActionRelatePieceParts",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateSystemData_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateSystemData",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateWorkLog",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
