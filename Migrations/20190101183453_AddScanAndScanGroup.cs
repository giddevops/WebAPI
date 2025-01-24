using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddScanAndScanGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScanGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    LastScanOccurredAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scan",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ScannerSerialNumber = table.Column<string>(nullable: false),
                    ScannerLabelId = table.Column<int>(nullable: false),
                    ScanGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scan_ScanGroup_ScanGroupId",
                        column: x => x.ScanGroupId,
                        principalTable: "ScanGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scan_ScannerLabel_ScannerLabelId",
                        column: x => x.ScannerLabelId,
                        principalTable: "ScannerLabel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scan_ScanGroupId",
                table: "Scan",
                column: "ScanGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Scan_ScannerLabelId",
                table: "Scan",
                column: "ScannerLabelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scan");

            migrationBuilder.DropTable(
                name: "ScanGroup");
        }
    }
}
