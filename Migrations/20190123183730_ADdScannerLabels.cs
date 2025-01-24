using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class ADdScannerLabels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabel");

            migrationBuilder.AlterColumn<int>(
                name: "ScannerLabelTypeId",
                table: "ScannerLabel",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "EndScannerLabel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    BarcodeGuid = table.Column<Guid>(nullable: true),
                    StartScannerLabelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndScannerLabel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndScannerLabel_ScannerLabel_StartScannerLabelId",
                        column: x => x.StartScannerLabelId,
                        principalTable: "ScannerLabel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EndScannerLabel_StartScannerLabelId",
                table: "EndScannerLabel",
                column: "StartScannerLabelId",
                unique: true,
                filter: "[StartScannerLabelId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabel",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabel");

            migrationBuilder.DropTable(
                name: "EndScannerLabel");

            migrationBuilder.AlterColumn<int>(
                name: "ScannerLabelTypeId",
                table: "ScannerLabel",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId",
                table: "ScannerLabel",
                column: "ScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
