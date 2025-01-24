using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class addmultiplescanneractionupdatesystemdatacommands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionUpdateSystemData_ScannerLabelType_ValueScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionUpdateSystemData_ValueScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropColumn(
                name: "ObjectField",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropColumn(
                name: "ObjectName",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropColumn(
                name: "TextValue",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.DropColumn(
                name: "ValueScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData");

            migrationBuilder.CreateTable(
                name: "ScannerActionUpdateSystemDataUpdateCommand",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScannerActionUpdateSystemDataUpdateCommand");

            migrationBuilder.AddColumn<string>(
                name: "FieldType",
                table: "ScannerActionUpdateSystemData",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjectField",
                table: "ScannerActionUpdateSystemData",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjectName",
                table: "ScannerActionUpdateSystemData",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextValue",
                table: "ScannerActionUpdateSystemData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValueScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateSystemData_ValueScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData",
                column: "ValueScannerLabelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionUpdateSystemData_ScannerLabelType_ValueScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData",
                column: "ValueScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
