using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddWOrkLogItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerStation_ScannerLabelType_DefaultScannerLabelTypeId",
                table: "ScannerStation");

            migrationBuilder.RenameColumn(
                name: "DefaultScannerLabelTypeId",
                table: "ScannerStation",
                newName: "DefaultScannerLabelId");

            migrationBuilder.RenameIndex(
                name: "IX_ScannerStation_DefaultScannerLabelTypeId",
                table: "ScannerStation",
                newName: "IX_ScannerStation_DefaultScannerLabelId");

            migrationBuilder.AddColumn<bool>(
                name: "Implied",
                table: "Scan",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "WorkLogItemActivityOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogItemActivityOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkLogItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    PerformedById = table.Column<int>(nullable: true),
                    StartDateTime = table.Column<DateTime>(nullable: true),
                    EndDateTime = table.Column<DateTime>(nullable: true),
                    InventoryItemId = table.Column<int>(nullable: true),
                    SalesOrderId = table.Column<int>(nullable: true),
                    RmaId = table.Column<int>(nullable: true),
                    ActivityText = table.Column<string>(nullable: true),
                    WorkLogItemActivityOptionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogItem_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkLogItem_Rma_RmaId",
                        column: x => x.RmaId,
                        principalTable: "Rma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkLogItem_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkLogItem_WorkLogItemActivityOption_WorkLogItemActivityOptionId",
                        column: x => x.WorkLogItemActivityOptionId,
                        principalTable: "WorkLogItemActivityOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogItem_InventoryItemId",
                table: "WorkLogItem",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogItem_RmaId",
                table: "WorkLogItem",
                column: "RmaId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogItem_SalesOrderId",
                table: "WorkLogItem",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogItem_WorkLogItemActivityOptionId",
                table: "WorkLogItem",
                column: "WorkLogItemActivityOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerStation_ScannerLabel_DefaultScannerLabelId",
                table: "ScannerStation",
                column: "DefaultScannerLabelId",
                principalTable: "ScannerLabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerStation_ScannerLabel_DefaultScannerLabelId",
                table: "ScannerStation");

            migrationBuilder.DropTable(
                name: "WorkLogItem");

            migrationBuilder.DropTable(
                name: "WorkLogItemActivityOption");

            migrationBuilder.DropColumn(
                name: "Implied",
                table: "Scan");

            migrationBuilder.RenameColumn(
                name: "DefaultScannerLabelId",
                table: "ScannerStation",
                newName: "DefaultScannerLabelTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ScannerStation_DefaultScannerLabelId",
                table: "ScannerStation",
                newName: "IX_ScannerStation_DefaultScannerLabelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerStation_ScannerLabelType_DefaultScannerLabelTypeId",
                table: "ScannerStation",
                column: "DefaultScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
