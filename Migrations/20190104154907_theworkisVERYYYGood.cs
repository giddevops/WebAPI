using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class theworkisVERYYYGood : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScannerActionUpdateLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ScannerEventId = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    InventoryItemScannerLabelTypeId = table.Column<int>(nullable: true)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScannerActionUpdateLocation");
        }
    }
}
