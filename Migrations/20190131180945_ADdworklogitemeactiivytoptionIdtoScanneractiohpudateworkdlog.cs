using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class ADdworklogitemeactiivytoptionIdtoScanneractiohpudateworkdlog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerLabelType_UserScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropTable(
                name: "InventoryItemWorkLogItem");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionUpdateWorkLog_UserScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropColumn(
                name: "EventName",
                table: "ScannerActionUpdateWorkLog");

            migrationBuilder.RenameColumn(
                name: "UserScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                newName: "WorkLogItemActivityOptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkLogItemActivityOptionId",
                table: "ScannerActionUpdateWorkLog",
                newName: "UserScannerLabelTypeId");

            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "ScannerActionUpdateWorkLog",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InventoryItemWorkLogItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreatedById = table.Column<int>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemWorkLogItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateWorkLog_UserScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                column: "UserScannerLabelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateWorkLog_ScannerLabelType_UserScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                column: "UserScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
