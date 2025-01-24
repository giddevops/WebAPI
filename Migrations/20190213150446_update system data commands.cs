using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class updatesystemdatacommands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScannerActionUpdateSystemDataUpdateCommand");

            migrationBuilder.CreateTable(
                name: "ScannerActionUpdateSystemDataCommand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    ScannerActionUpdateSystemDataId = table.Column<int>(nullable: true),
                    ObjectName = table.Column<string>(nullable: true),
                    ObjectField = table.Column<string>(nullable: true),
                    FieldType = table.Column<string>(nullable: true),
                    ValueScannerLabelTypeId = table.Column<int>(nullable: true),
                    TextValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerActionUpdateSystemDataCommand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateSystemDataCommand_ScannerActionUpdateSystemData_ScannerActionUpdateSystemDataId",
                        column: x => x.ScannerActionUpdateSystemDataId,
                        principalTable: "ScannerActionUpdateSystemData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateSystemDataCommand_ScannerLabelType_ValueScannerLabelTypeId",
                        column: x => x.ValueScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateSystemDataCommand_ScannerActionUpdateSystemDataId",
                table: "ScannerActionUpdateSystemDataCommand",
                column: "ScannerActionUpdateSystemDataId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateSystemDataCommand_ValueScannerLabelTypeId",
                table: "ScannerActionUpdateSystemDataCommand",
                column: "ValueScannerLabelTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScannerActionUpdateSystemDataCommand");

            migrationBuilder.CreateTable(
                name: "ScannerActionUpdateSystemDataUpdateCommand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    FieldType = table.Column<string>(nullable: true),
                    ObjectField = table.Column<string>(nullable: true),
                    ObjectName = table.Column<string>(nullable: true),
                    ScannerActionUpdateSystemDataId = table.Column<int>(nullable: true),
                    TextValue = table.Column<string>(nullable: true),
                    ValueScannerLabelTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerActionUpdateSystemDataUpdateCommand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateSystemDataUpdateCommand_ScannerActionUpdateSystemData_ScannerActionUpdateSystemDataId",
                        column: x => x.ScannerActionUpdateSystemDataId,
                        principalTable: "ScannerActionUpdateSystemData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateSystemDataUpdateCommand_ScannerLabelType_ValueScannerLabelTypeId",
                        column: x => x.ValueScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateSystemDataUpdateCommand_ScannerActionUpdateSystemDataId",
                table: "ScannerActionUpdateSystemDataUpdateCommand",
                column: "ScannerActionUpdateSystemDataId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateSystemDataUpdateCommand_ValueScannerLabelTypeId",
                table: "ScannerActionUpdateSystemDataUpdateCommand",
                column: "ValueScannerLabelTypeId");
        }
    }
}
