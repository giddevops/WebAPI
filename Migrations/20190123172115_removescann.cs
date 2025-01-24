using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class removescann : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerEvent_ScannerEventId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateLocation_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateSystemData_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropTable(
                name: "ScannerEventScannerLabelType");

            migrationBuilder.DropTable(
                name: "ScannerEvent");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionUpdateWorkLog_ScannerEventId",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionUpdateSystemData_ScannerEventId",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionUpdateLocation_ScannerEventId",
                table: "ScannerActionUpdateLocation");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionRelatePieceParts_ScannerEventId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.RenameColumn(
                name: "ScannerEventId",
                table: "ScannerActionUpdateWorkLog",
                newName: "ScannerLabelTypeId");

            migrationBuilder.RenameColumn(
                name: "ScannerEventId",
                table: "ScannerActionUpdateSystemData",
                newName: "ScannerLabelTypeId");

            migrationBuilder.RenameColumn(
                name: "ScannerEventId",
                table: "ScannerActionUpdateLocation",
                newName: "ScannerLabelTypeId");

            migrationBuilder.RenameColumn(
                name: "ScannerEventId",
                table: "ScannerActionRelatePieceParts",
                newName: "ScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateWorkLog_ScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                column: "ScannerLabelTypeId",
                unique: true,
                filter: "[ScannerLabelTypeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateSystemData_ScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData",
                column: "ScannerLabelTypeId",
                unique: true,
                filter: "[ScannerLabelTypeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateLocation_ScannerLabelTypeId",
                table: "ScannerActionUpdateLocation",
                column: "ScannerLabelTypeId",
                unique: true,
                filter: "[ScannerLabelTypeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionRelatePieceParts_ScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                column: "ScannerLabelTypeId",
                unique: true,
                filter: "[ScannerLabelTypeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateLocation_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerActionUpdateLocation",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateSystemData_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateLocation_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerActionUpdateLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateSystemData_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionUpdateWorkLog_ScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionUpdateSystemData_ScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionUpdateLocation_ScannerLabelTypeId",
                table: "ScannerActionUpdateLocation");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionRelatePieceParts_ScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.RenameColumn(
                name: "ScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                newName: "ScannerEventId");

            migrationBuilder.RenameColumn(
                name: "ScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData",
                newName: "ScannerEventId");

            migrationBuilder.RenameColumn(
                name: "ScannerLabelTypeId",
                table: "ScannerActionUpdateLocation",
                newName: "ScannerEventId");

            migrationBuilder.RenameColumn(
                name: "ScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                newName: "ScannerEventId");

            migrationBuilder.CreateTable(
                name: "ScannerEvent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ScannerEventLabelTypeId = table.Column<int>(nullable: false),
                    ScannerId = table.Column<int>(nullable: true),
                    ScannerStationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerEvent", x => x.Id);
                    table.UniqueConstraint("AK_ScannerEvent_ScannerEventLabelTypeId", x => x.ScannerEventLabelTypeId);
                    table.ForeignKey(
                        name: "FK_ScannerEvent_ScannerLabelType_ScannerEventLabelTypeId",
                        column: x => x.ScannerEventLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScannerEventScannerLabelType",
                columns: table => new
                {
                    ScannerEventId = table.Column<int>(nullable: false),
                    ScannerLabelTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerEventScannerLabelType", x => new { x.ScannerEventId, x.ScannerLabelTypeId });
                    table.ForeignKey(
                        name: "FK_ScannerEventScannerLabelType_ScannerEvent_ScannerEventId",
                        column: x => x.ScannerEventId,
                        principalTable: "ScannerEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScannerEventScannerLabelType_ScannerLabelType_ScannerLabelTypeId",
                        column: x => x.ScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateWorkLog_ScannerEventId",
                table: "ScannerActionUpdateWorkLog",
                column: "ScannerEventId",
                unique: true,
                filter: "[ScannerEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateSystemData_ScannerEventId",
                table: "ScannerActionUpdateSystemData",
                column: "ScannerEventId",
                unique: true,
                filter: "[ScannerEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateLocation_ScannerEventId",
                table: "ScannerActionUpdateLocation",
                column: "ScannerEventId",
                unique: true,
                filter: "[ScannerEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionRelatePieceParts_ScannerEventId",
                table: "ScannerActionRelatePieceParts",
                column: "ScannerEventId",
                unique: true,
                filter: "[ScannerEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerEventScannerLabelType_ScannerLabelTypeId",
                table: "ScannerEventScannerLabelType",
                column: "ScannerLabelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerEvent_ScannerEventId",
                table: "ScannerActionRelatePieceParts",
                column: "ScannerEventId",
                principalTable: "ScannerEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateLocation_ScannerEvent_ScannerEventId",
                table: "ScannerActionUpdateLocation",
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
    }
}
